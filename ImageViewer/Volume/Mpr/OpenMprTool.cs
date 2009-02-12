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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	[MenuAction("openMpr", "imageviewer-contextmenu/MenuVolume/MenuOpenMpr", "OpenMpr")]
	[IconSet("OpenMpr", IconScheme.Colour, "Icons.OpenToolSmall.png", "Icons.OpenToolMedium.png", "Icons.OpenToolLarge.png")]
	[EnabledStateObserver("openMpr", "OpenMprEnabled", "OpenMprEnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class OpenMprTool : Tool<IImageViewerToolContext>
	{
		private bool _openMprEnabled;

		private event EventHandler _openMprEnabledChanged;

		public OpenMprTool()
		{
			_openMprEnabled = false;
		}

		public bool OpenMprEnabled
		{
			get { return _openMprEnabled; }
			set
			{
				if (value == _openMprEnabled)
					return;

				_openMprEnabled = value;
				EventsHelper.Fire(_openMprEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler OpenMprEnabledChanged
		{
			add { _openMprEnabledChanged += value; }
			remove { _openMprEnabledChanged -= value; }
		}

		public override void Initialize()
		{
			base.Initialize();

			Context.Viewer.EventBroker.ImageBoxSelected += OnImageBoxSelected;
			Context.Viewer.EventBroker.DisplaySetSelected += OnDisplaySetSelected;
		}

		protected override void Dispose(bool disposing)
		{
			Context.Viewer.EventBroker.ImageBoxSelected -= OnImageBoxSelected;
			Context.Viewer.EventBroker.DisplaySetSelected -= OnDisplaySetSelected;

			base.Dispose(disposing);
		}

		public void OpenMpr()
		{
			try
			{
				//host component as workspace.
				VolumeLoader loader = new VolumeLoader();
				//ggerade ToDo: If DisplaySet does not contain all compatible images, use the selected pres image and find the
				//	images that are compatible with it
				Volume volume = loader.LoadFromDisplaySet(this.Context.Viewer.SelectedPresentationImage.ParentDisplaySet);
				if (volume == null)
					return;

				ImageViewerComponent component = MprLayoutManager.CreateMprLayoutAndComponent(volume);

				ImageViewerComponent.LaunchInActiveWindow(component, SR.MprWorkspaceTitlePrefix);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		private void OnImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
		{
			if (e.SelectedImageBox.DisplaySet == null)
				UpdateEnabled(null);
		}

		private void OnDisplaySetSelected(object sender, DisplaySetSelectedEventArgs e)
		{
			UpdateEnabled(e.SelectedDisplaySet);
		}

		private void UpdateEnabled(IDisplaySet selectedDisplaySet)
		{
			if (selectedDisplaySet == null || selectedDisplaySet.PresentationImages.Count < 2)
			{
				OpenMprEnabled = false;
			}
			else
			{
				OpenMprEnabled = true;
			}
		}
	}
}
