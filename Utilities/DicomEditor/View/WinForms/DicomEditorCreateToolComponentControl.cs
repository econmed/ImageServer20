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
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Utilities.DicomEditor.Tools;

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomEditorCreateToolComponent"/>
    /// </summary>
    public partial class DicomEditorCreateToolComponentControl : CustomUserControl
    {
        private DicomEditorCreateToolComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomEditorCreateToolComponentControl(DicomEditorCreateToolComponent component)
        {
            InitializeComponent();

            _component = component;

            _group.DataBindings.Add("Text", _component, "Group", true, DataSourceUpdateMode.OnPropertyChanged);
            _element.DataBindings.Add("Text", _component, "Element", true, DataSourceUpdateMode.OnPropertyChanged);
            _tagName.DataBindings.Add("Value", _component, "TagName", true, DataSourceUpdateMode.OnPropertyChanged);
            _vr.DataBindings.Add("Value", _component, "Vr", true, DataSourceUpdateMode.OnPropertyChanged);
            _vr.DataBindings.Add("Enabled", _component, "VrEnabled", true, DataSourceUpdateMode.Never);
            _value.DataBindings.Add("Value", _component, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
            _accept.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.Never);

			base.AcceptButton = _accept;
			base.CancelButton = _cancel;
        }

        private void _accept_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancel_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
