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
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomServerEditComponent"/>
    /// </summary>
    public partial class DicomServerEditComponentControl : CustomUserControl
    {
        private DicomServerEditComponent _component;
        private BindingSource _bindingSource;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerEditComponentControl(DicomServerEditComponent component)
        {
            InitializeComponent();

            _component = component;

			this.AcceptButton = _btnAccept;
			this.CancelButton = _btnCancel;

            AcceptClicked += new EventHandler(OnAcceptClicked);
            CancelClicked += new EventHandler(OnCancelClicked);

            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _component;
            this._serverName.DataBindings.Add("Text", _bindingSource, "ServerName", true, DataSourceUpdateMode.OnPropertyChanged);
            this._location.DataBindings.Add("Text", _bindingSource, "ServerLocation", true, DataSourceUpdateMode.OnPropertyChanged);
            this._ae.DataBindings.Add("Text", _bindingSource, "ServerAE", true, DataSourceUpdateMode.OnPropertyChanged);
            this._host.DataBindings.Add("Text", _bindingSource, "ServerHost", true, DataSourceUpdateMode.OnPropertyChanged);
            this._port.DataBindings.Add("Text", _bindingSource, "ServerPort", true, DataSourceUpdateMode.OnPropertyChanged);
            _btnAccept.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            this._serverName.DataBindings.Add("Readonly", _component, "FieldReadonly", true, DataSourceUpdateMode.OnPropertyChanged);
            this._host.DataBindings.Add("Readonly", _component, "FieldReadonly", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        public event EventHandler AcceptClicked
        {
            add { _btnAccept.Click += value; }
            remove { _btnAccept.Click -= value; }
        }

        public event EventHandler CancelClicked
        {
            add { _btnCancel.Click += value; }
            remove { _btnCancel.Click -= value; }
        }

        private void OnAcceptClicked(object sender, EventArgs e)
        {
			_component.Accept();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
