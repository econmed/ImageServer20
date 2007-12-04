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

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A host for components that are children of other components.
	/// </summary>
    public class ChildComponentHost : ApplicationComponentHost
    {
        private IApplicationComponentHost _parentHost;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parentHost">The object that hosts the <paramref name="childComponent"/>'s parent component.</param>
		/// <param name="childComponent">The child application component being hosted.</param>
        public ChildComponentHost(IApplicationComponentHost parentHost, IApplicationComponent childComponent)
            : base(childComponent)
        {
            Platform.CheckForNullReference(parentHost, "parentHost");

            _parentHost = parentHost;
        }

		/// <summary>
		/// Gets the <see cref="DesktopWindow"/> that owns the parent component.
		/// </summary>
        public override DesktopWindow DesktopWindow
        {
            get { return _parentHost.DesktopWindow; }
        }

		/// <summary>
		/// Gets the title of the parent host.
		/// </summary>
        public override string Title
        {
            get { return _parentHost.Title; }
        }

    }
}
