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
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    /// <summary>
    /// Represents handler that handles requests with RequestType of "WADO"
    /// </summary>
    [ExtensionOf(typeof(WADORequestTypeExtensionPoint))]
    class StandardWADORequestTypeHandler : IWADORequestTypeHandler
    {

        #region IWADORequestTypeHandler Members

        public string RequestType
        {
            get { return "WADO"; }
        }

        public void Validate(HttpListenerRequest request)
        {
            string studyUid = request.QueryString["studyUID"];
            string seriesUid = request.QueryString["seriesUid"];
            string objectUid = request.QueryString["objectUid"];

            if (String.IsNullOrEmpty(studyUid))
            {
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("studyUID parameter is required"));
            }

            if (String.IsNullOrEmpty(seriesUid))
            {
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("seriesUid parameter is required"));
                
            }

            if (String.IsNullOrEmpty(objectUid))
            {
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("objectUid parameter is required"));
            }
        }
        
        #endregion

        #region IWADORequestTypeHandler Members


        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion

        #region IWADORequestTypeHandler Members


        public WADOResponse Process(WADORequestTypeHandlerContext context)
        {
            //Validate(context.HttpContext.Request);

            ObjectStreamingHandlerFactory factory = new ObjectStreamingHandlerFactory();
            IObjectStreamingHandler handler = factory.CreateHandler(context.HttpContext.Request);
            return handler.Process(context);
        }

        #endregion
    }
}
