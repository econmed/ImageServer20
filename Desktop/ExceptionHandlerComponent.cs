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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="ExceptionHandlerComponent"/>.
    /// </summary>
    [ExtensionPoint]
	public sealed class ExceptionHandlerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// Application component used for reporting exceptions to the user.
    /// </summary>
    /// <remarks>
    /// This class is intended for internal framework use only.
    /// </remarks>
    [AssociateView(typeof(ExceptionHandlerComponentViewExtensionPoint))]
    public class ExceptionHandlerComponent : ApplicationComponent
    {
        private Exception _exception;
        private string _message;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ExceptionHandlerComponent(Exception e, string message)
        {
            _exception = e;
            _message = message;
        }

        ///<summary>
        /// The exception to be shown to the user.
        ///</summary>
        public Exception Exception
        {
            get { return _exception; }
        }

		/// <summary>
		/// A user-friendly message to display.
		/// </summary>
        public string Message
        {
            get { return _message; }
        }

		/// <summary>
		/// The user has dismissed the component in the view.
		/// </summary>
        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }
    }
}
