#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.DicomServices.ServiceModel.Streaming;
using ClearCanvas.ImageServer.Services.Streaming.HeaderRetrieval;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.Services.Streaming.Shreds
{
	/// <summary>
	/// Plugin to handle streaming request for the ImageServer.
	/// </summary>
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class HeaderStreamingServer : WcfShred
	{
		#region Private Members

		private readonly string _className;

		#endregion

		#region Constructors

		public HeaderStreamingServer()
		{
			_className = GetType().ToString();
		}

		#endregion

		#region IShred Implementation Shred Override

		public override void Start()
		{
			Platform.Log(LogLevel.Debug, "{0}[{1}]: Start invoked", _className, AppDomain.CurrentDomain.FriendlyName);

			HeaderStreamingServerSettings settings = HeaderStreamingServerSettings.Default;

			try
			{
				if (settings.BindingType == "wshttp")
				{
					Platform.Log(LogLevel.Info, "Starting {0} using WS Http binding", GetDisplayName());
					StartHttpHost<HeaderRetrievalService, IHeaderRetrievalService>("HeaderRetrieval", GetDescription());
				}
				else if (settings.BindingType == "http")
				{
					Platform.Log(LogLevel.Info, "Starting {0} using basic Http binding", GetDisplayName()); 
					StartBasicHttpHost<HeaderRetrievalService, IHeaderRetrievalService>("HeaderRetrieval", GetDescription());
				}
				else
				{
					Platform.Log(LogLevel.Info, "Starting {0} using NET TCP binding", GetDisplayName()); 
					StartNetTcpHost<HeaderRetrievalService, IHeaderRetrievalService>("HeaderRetrieval", GetDescription());
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Fatal, e, "Unexpected exception starting Streaming Server Shred");
				throw;
			}
		}

		public override void Stop()
		{
			Platform.Log(LogLevel.Info, "{0}[{1}]: Stop invoked", _className, AppDomain.CurrentDomain.FriendlyName);
			StopHost("HeaderRetrieval");
		}

		public override string GetDisplayName()
		{
			return SR.HeaderStreamingServerDisplayName;
		}

		public override string GetDescription()
		{
			return SR.HeaderStreamingServerDescription;
		}

		#endregion
	}
}