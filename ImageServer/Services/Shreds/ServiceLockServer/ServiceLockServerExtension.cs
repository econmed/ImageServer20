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
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.Services.Shreds.ServiceLockServer
{
    /// <summary>
    /// Plugin to handle ServiceLock processing for the ImageServer.
    /// </summary>
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class ServiceLockServerExtension : WcfShred
    {
        private readonly string _className;
        private readonly string _servicesServerEndpointName;

        public ServiceLockServerExtension()
        {
            _className = this.GetType().ToString();
            _servicesServerEndpointName = "ServicesServer";
        }
        public override void Start()
        {
            Platform.Log(LogLevel.Info, "{0}[{1}]: Start invoked", _className, AppDomain.CurrentDomain.FriendlyName);

            ServiceLockServerManager.Instance.Start();
        }

        public override void Stop()
        {
            StopHost(_servicesServerEndpointName);

            ServiceLockServerManager.Instance.Stop();

            Platform.Log(LogLevel.Info, "{0}[{1}]: Stop invoked", _className, AppDomain.CurrentDomain.FriendlyName);
        }

        public override string GetDisplayName()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetDescription()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}