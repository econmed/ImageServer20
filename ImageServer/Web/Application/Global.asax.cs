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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application
{
    public class Global : System.Web.HttpApplication
    {
        private DateTime start;

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            start = DateTime.Now;
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["PerformanceLogging"].Equals("true"))
            {
                //Ignore some of the requests
                if (Request.Url.AbsoluteUri.Contains("PersistantImage.ashx") ||
                    Request.Url.AbsoluteUri.Contains("WebResource.axd") ||
                    Request.Url.AbsoluteUri.Contains("ScriptResource.axd") ||
                    Request.Url.AbsoluteUri.Contains("Pages/Login") ||
                    Request.Url.AbsoluteUri.Contains("Pages/Error") ||
                    Request.Url.AbsoluteUri.Contains("&error=")) return;
                TimeSpan elapsedTime = DateTime.Now.Subtract(start);
                string processingTime = elapsedTime.Minutes + ":" + elapsedTime.Seconds + ":" + elapsedTime.Milliseconds;

                string userName = "Not Logged In.";
                if (SessionManager.Current != null)
                {
                    userName = SessionManager.Current.User.Credentials.UserName;
                }
                Platform.Log(LogLevel.Debug,
                             string.Format("USER: {0} URL: {1} PROCESSING TIME: {2}", userName,
                                           this.Request.Url.AbsoluteUri, processingTime));
            }
        }
    }
}