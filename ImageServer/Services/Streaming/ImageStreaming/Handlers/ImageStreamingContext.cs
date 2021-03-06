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

using System.Collections.Specialized;
using System.IO;
using System.Net;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    public class ImageStreamingContext : StreamingContext
    {
        private DicomPixelData _pd;
        private string _serverAE;
        private int _frameNumber;
        private readonly string _nextSeriesUid;
        private readonly string _nextSopUid;

        #region PERFORMANCE TESTING STUFF
        private static DicomPixelData _testCompressedImage;
        private static DicomPixelData _testUncompressedImage;
        private readonly bool testCompressed;
        private readonly bool testUncompressed;
        #endregion

        public ImageStreamingContext(HttpListenerContext context)
        {
            Request = context.Request;
            Response = context.Response;
            NameValueCollection query = Request.QueryString;

            #region INIT STUFF FOR PERFORMANCE TESTING
            #if DEBUG 

            if (query["testcompressed"] != null)
            {
                testCompressed= true;
            }
            else if (query["testuncompressed"] != null)
            {
                testUncompressed = true;
            }
            if (_testCompressedImage == null)
            {
                using (Stream stream = typeof(ImageStreamingContext).Assembly.GetManifestResourceStream("ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Test.TestSamples.compressed.dcm"))
                {
                    DicomFile file = new DicomFile();
                    file.Load(stream);

                    _testCompressedImage = DicomPixelData.CreateFrom(file);
                }
                
            }

            if (_testUncompressedImage == null)
            {
                using (Stream stream = typeof(ImageStreamingContext).Assembly.GetManifestResourceStream("ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Test.TestSamples.uncompressed.dcm"))
                {
                    DicomFile file = new DicomFile();
                    file.Load(stream);

                    _testUncompressedImage = DicomPixelData.CreateFrom(file);
                }
                
            }
            #endif

            #endregion

            _frameNumber = 0;
            if (query["FrameNumber"] != null)
                int.TryParse(query["FrameNumber"], out _frameNumber);

            _nextSeriesUid = query["nextSeriesUid"];
            _nextSopUid = query["nextObjectUid"];

        }
       
        
        public string ImagePath
        {
            get
            {
                return StorageLocation.GetSopInstancePath(SeriesInstanceUid, ObjectUid);
            }
        }
        
        public DicomPixelData PixelData
        {
            get
            {
                #region PERFORMANCE TESTING CODE
                #if DEBUG 
                // If requested, the test images will be streamed directly from memory
                if (testCompressed)
                {
                    return _testCompressedImage;
                }
                else if (testUncompressed)
                {
                    return _testUncompressedImage;
                } 
                #endif
                #endregion

                if (_pd == null)
                {
                    PixelDataManager manager = PixelDataManager.GetInstance(StorageLocation);
                    _pd = manager.GetPixelData(SeriesInstanceUid, ObjectUid, _nextSeriesUid, _nextSopUid);
                }
                return _pd;
            }
        }

        public bool IsMultiFrame
        {
            get
            {
                return PixelData.NumberOfFrames > 1;
            }
        }

        public string ServerAE
        {
            get { return _serverAE; }
            set { _serverAE = value; }
        }

        public int FrameNumber
        {
            get { return _frameNumber; }
            set { _frameNumber = value; }
        }

    }
}
