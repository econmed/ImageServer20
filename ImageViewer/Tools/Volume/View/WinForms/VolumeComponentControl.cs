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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Volume.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="VolumeComponent"/>
    /// </summary>
    public partial class VolumeComponentControl : CustomUserControl
    {
		private BindingSource _bindingSource;
		private VolumeComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public VolumeComponentControl(VolumeComponent component)
        {
			_component = component;

			InitializeComponent();

			AddDefaultTabs();

			this._createVolumeButton.Click += new EventHandler(OnCreateVolumeButtonClick);

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _component;

			_component.SubjectChanged += new EventHandler(OnSubjectChanged);
			_createVolumeButton.DataBindings.Add("Enabled", _bindingSource, "CreateVolumeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_tabControl.DataBindings.Add("Enabled", _bindingSource, "VolumeSettingsEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		void AddDefaultTabs()
		{
			for (int i = 0; i < 2; i++)
			{
				TabPage tabPage = new TabPage("Tissue");
				TissueControl control = new TissueControl();
				tabPage.Controls.Add(control);
				control.Dock = DockStyle.Fill;
				_tabControl.TabPages.Add(tabPage);
			}
		}

		void OnCreateVolumeButtonClick(object sender, EventArgs e)
		{
			_component.CreateVolume();
		}

		void OnSubjectChanged(object sender, EventArgs e)
		{
			_bindingSource.ResetBindings(false);

			UpdateTabControl();
		}

		private void UpdateTabControl()
		{
			if (_component.VolumeGraphics == null)
				return;

			int i = 0;

			foreach (Graphic layer in _component.VolumeGraphics)
			{
				VolumeGraphic volumeLayer = layer as VolumeGraphic;

				if (volumeLayer != null)
				{
					TabPage page = _tabControl.TabPages[i];
					TissueControl control = page.Controls[0] as TissueControl;

					if (control != null)
						control.TissueSettings = volumeLayer.TissueSettings;
				}

				i++;
			}
		}
    }
}
