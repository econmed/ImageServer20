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
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	[ExtensionOf(typeof(MagnificationViewExtensionPoint))]
    public class MagnificationComponentView : WinFormsView, IMagnificationView
    {
        private MagnificationForm _form;

		public MagnificationComponentView()
		{
		}

		public override object GuiElement
        {
            get { throw new InvalidOperationException("Not valid for this view type."); }
        }

		#region IMagnificationView Members

		public void Open(float magnificationFactor, PresentationImage image, IMouseInformation mouseInformation)
		{
			_form = new MagnificationForm(magnificationFactor, image, mouseInformation.Location);
			_form.Show();
		}

		public void Close()
		{
			if (_form != null)
			{
				_form.Dispose();
				_form = null;
			}
		}

		public void UpdateMouseInformation(IMouseInformation mouseInformation)
		{
			if (_form == null)
				throw new InvalidOperationException("Open must be called before UpdateMouseInformation");

			_form.UpdateMousePosition(mouseInformation.Location);
		}

		#endregion
	}
}