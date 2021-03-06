﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Streaming.Shreds;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    
    /// <summary>
    /// Represents a handler that can stream dicom images to web clients.
    /// </summary>
    internal class ImageStreamingHandler : IObjectStreamingHandler
    {
    	// cache the extension for performance purpose
        static readonly Dictionary<string, Type> _processorMap = new Dictionary<string, Type>();
        
        public WADOResponse Process(WADORequestTypeHandlerContext context)
        {
            Platform.CheckForNullReference(context, "httpContext");
            Platform.CheckForNullReference(context.ServerAE, "context.ServerAE");
            Platform.CheckForNullReference(context.HttpContext, "context.HttpContext");

            // Cache the query string for performance purpose. Every time Request.QueryString is called, .NET will rebuild the entire dictionary.
            NameValueCollection query = context.HttpContext.Request.QueryString;

            ServerPartition partition = ServerPartitionMonitor.Instance.GetPartition(context.ServerAE);
            if (partition== null)
				throw new WADOException(HttpStatusCode.NotFound, String.Format(SR.FaultPartitionNotExists, context.ServerAE));

            if (!partition.Enabled)
                throw new WADOException(HttpStatusCode.Forbidden, String.Format(SR.FaultPartitionDisabled, context.ServerAE));
            
            ImageStreamingContext streamingContext = new ImageStreamingContext(context.HttpContext);
            streamingContext.ServerAE = context.ServerAE;
            streamingContext.ContentType = query["ContentType"];
            streamingContext.AcceptTypes = context.HttpContext.Request.AcceptTypes;
            streamingContext.StudyInstanceUid = query["studyuid"];
            streamingContext.SeriesInstanceUid = query["seriesuid"];
            streamingContext.ObjectUid = query["objectuid"];

            string sessionId = context.HttpContext.Request.RemoteEndPoint.Address.ToString();
            StudyStorageLoader storageLoader = new StudyStorageLoader(sessionId);
            storageLoader.CacheEnabled = ImageStreamingServerSettings.Default.EnableCache;
            storageLoader.CacheRetentionTime = ImageStreamingServerSettings.Default.CacheRetentionWindow;
            streamingContext.StorageLocation = storageLoader.Find(streamingContext.StudyInstanceUid, partition);
            
            // convert the dicom image into the appropriate mime type
            WADOResponse response = new WADOResponse();
            IImageMimeTypeProcessor processor = GetMimeTypeProcessor(streamingContext);

            MimeTypeProcessorOutput output = processor.Process(streamingContext);   
            response.Output = output.Output;
            response.ContentType = output.ContentType;
            response.IsLast = output.IsLast;
            
            return response;
        }

        protected static bool ClientAcceptable(ImageStreamingContext context, string contentType)
        {
            if (context.AcceptTypes == null)
                return false;
            
            foreach(string rawmime in context.AcceptTypes)
            {
                string mime = rawmime;
                if (rawmime.Contains(";"))
                    mime = rawmime.Substring(0, rawmime.IndexOf(";"));

                if (mime=="*/*" || mime==contentType)
                    return true;
            }

            return false;
        }
        protected virtual IImageMimeTypeProcessor GetMimeTypeProcessor(ImageStreamingContext context)
        {
            string responseContentType = context.ContentType;
            if (String.IsNullOrEmpty(responseContentType))
            {
                if (context.IsMultiFrame)
                    responseContentType = "application/dicom";
                else
                {
                    responseContentType = "image/jpeg";
                    if (!ClientAcceptable(context, responseContentType))
                    {
                        responseContentType = "application/dicom";
                    }
                }
            }

            if (_processorMap.ContainsKey(context.ContentType))
            {
                return (IImageMimeTypeProcessor) Activator.CreateInstance(_processorMap[context.ContentType]);
            }

            ImageMimeTypeProcessorExtensionPoint xp = new ImageMimeTypeProcessorExtensionPoint();
            object[] plugins = xp.CreateExtensions();

            bool found = false;
            foreach (IImageMimeTypeProcessor mimeTypeConverter in plugins)
            {

                if (mimeTypeConverter.OutputMimeType == responseContentType)
                {
                    found = true;
                    _processorMap.Add(context.ContentType, mimeTypeConverter.GetType());

                    if (ClientAcceptable(context, mimeTypeConverter.OutputMimeType))
                    {
                        return mimeTypeConverter;
                    }
                }
            }

            if (found)
            {
                throw new WADOException(HttpStatusCode.NotAcceptable,
                                    String.Format("{0} is supported but is not acceptable by the client", responseContentType));
            }
            else
            {
                throw new WADOException(HttpStatusCode.BadRequest,
                                    String.Format("The specified contentType '{0}' is not supported", responseContentType));
            }
        }

    }
}