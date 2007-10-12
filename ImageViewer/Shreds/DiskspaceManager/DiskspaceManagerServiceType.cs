#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.DiskspaceManager;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
	[Serializable]
	internal class DiskspaceManagerException : Exception
	{
		public DiskspaceManagerException(string message)
			: base(message)
		{
		}

		protected DiskspaceManagerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ 
		}
	}
	
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public sealed class DiskspaceManagerServiceType : IDiskspaceManagerService
    {
        public DiskspaceManagerServiceType()
        {
		}

		#region IDiskspaceManagerService Members

		public DiskspaceManagerServiceInformation GetServiceInformation()
		{
			try
			{
				return DiskspaceManagerProcessor.Instance.GetServiceInformation();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				//we throw a serializable, non-FaultException-derived exception so that the 
				//client channel *does* get closed.
				string message = SR.ExceptionFailedToGetServerConfiguration;
				message += "\nDetail: " + e.Message;
				throw new DiskspaceManagerException(message);
			}
		}

		public void UpdateServiceConfiguration(DiskspaceManagerServiceConfiguration newConfiguration)
		{
			try
			{
				DiskspaceManagerProcessor.Instance.UpdateServiceConfiguration(newConfiguration);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				//we throw a serializable, non-FaultException-derived exception so that the 
				//client channel *does* get closed.
				string message = SR.ExceptionFailedToUpdateServerConfiguration;
				message += "\nDetail: " + e.Message;
				throw new DiskspaceManagerException(message);
			}
		}

		#endregion
	}
}
