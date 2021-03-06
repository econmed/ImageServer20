﻿#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Collections;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Volume.Mpr.Tools;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	//TODO: try using the new ViewerSetupHelper class instead.

	public class MprViewerComponent : ImageViewerComponent
	{
		#region Private fields

		private ObservableDisposableList<IMprVolume> _volumes;
		private IMprWorkspace _mprWorkspace;

		private string _title;

		#endregion

		public MprViewerComponent(Volume volume) : this()
		{
			_volumes.Add(new MprVolume(volume));
		}

		public MprViewerComponent(IMprVolume volume) : this()
		{
			_volumes.Add(volume);
		}

		public MprViewerComponent() : base(new MprLayoutManager(), null)
		{
			_volumes = new ObservableDisposableList<IMprVolume>();
			_volumes.EnableEvents = true;

			_mprWorkspace = new BasicMprWorkspace(this);
		}

		public IMprWorkspace MprWorkspace
		{
			get { return _mprWorkspace; }
		}

		public IObservableList<IMprVolume> Volumes
		{
			get { return _volumes; }
		}

		public string Title
		{
			get
			{
				if (_title == null)
					_title = this.SuggestTitle();
				return _title;
			}
			set
			{
				if (_title != value)
				{
					_title = value;
					base.NotifyPropertyChanged("Title");
				}
			}
		}

		protected virtual string SuggestTitle()
		{
			return string.Format(SR.FormatMprWorkspaceTitle, StringUtilities.Combine(this.Volumes, String.Format(" {0} ", SR.VolumeLabelSeparator), delegate(IMprVolume volume) { return volume.Description; }));
		}

		protected override IEnumerable CreateTools()
		{
			ArrayList results = new ArrayList();
			foreach (object tool in base.CreateTools())
				results.Add(tool);

			foreach (object tool in new MprViewerToolExtensionPoint().CreateExtensions())
				results.Add(tool);
			
			return results;
		}

		protected override ImageViewerToolContext CreateToolContext()
		{
			return new MprViewerToolContext(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_mprWorkspace != null)
				{
					_mprWorkspace.Dispose();
					_mprWorkspace = null;
				}

				if (_volumes != null)
				{
					_volumes.Dispose();
					_volumes = null;
				}
			}

			base.Dispose(disposing);
		}

		#region Tool Context

		private class MprViewerToolContext : ImageViewerToolContext, IMprViewerToolContext
		{
			public MprViewerToolContext(MprViewerComponent viewer)
				: base(viewer)
			{
			}

			public new MprViewerComponent Viewer
			{
				get { return (MprViewerComponent)base.Viewer; }
			}
		}

		#endregion

		#region MprWorkspace

		private class BasicMprWorkspace : IMprWorkspace
		{
			private event EventHandler _drawing;
			private MprViewerComponent _mprViewer;
			private ObservableList<IImageSet> _imageSets = new ObservableList<IImageSet>();

			public BasicMprWorkspace(MprViewerComponent mprViewer)
			{
				_mprViewer = mprViewer;
			}

			public void Dispose()
			{
				if (_imageSets != null)
				{
					foreach (IImageSet imageSet in _imageSets)
					{
						imageSet.Dispose();
					}
					_imageSets.Clear();
					_imageSets = null;
				}

				_mprViewer = null;
			}

			public MprViewerComponent MprViewer
			{
				get { return _mprViewer; }
			}

			public ObservableList<IImageSet> ImageSets
			{
				get { return _imageSets; }
			}

			public event EventHandler Drawing
			{
				add { _drawing += value; }
				remove { _drawing -= value; }
			}

			public void Draw()
			{
				EventsHelper.Fire(_drawing, this, EventArgs.Empty);
				foreach (IImageSet imageSet in _imageSets)
				{
					imageSet.Draw();
				}
			}
		}

		#endregion

		#region Layout Manager

		private class MprLayoutManager : LayoutManager
		{
			private bool _layoutCompleted = false;

			public override void Layout()
			{
				//The LaunchMprTool calls layout b/c it could take a while, but then
				//ImageViewerComponent.Launch calls it again.
				if (_layoutCompleted)
				{
					this.ImageViewer.PhysicalWorkspace.Draw();
					return;
				}

				_layoutCompleted = true;

				this.BuildLogicalWorkspace();
				this.LayoutPhysicalWorkspace();
				this.FillPhysicalWorkspace();
				this.ImageViewer.PhysicalWorkspace.Draw();
			}

			public new MprViewerComponent ImageViewer
			{
				get { return (MprViewerComponent) base.ImageViewer; }
			}

			protected virtual IDisplaySet CreateDisplaySet(int number, IMprSliceSet sliceSet)
			{
				string name;
				if (sliceSet is IMprStandardSliceSet && ((IMprStandardSliceSet)sliceSet).IsReadOnly)
					name = string.Format(SR.FormatMprDisplaySetName, sliceSet.Description);
				else
					name = string.Format(SR.FormatMprDisplaySetName, number - 1);

				DisplaySet displaySet = new MprDisplaySet(name, sliceSet);
				displaySet.Description = name;
				displaySet.Number = number;
				return displaySet;
			}

			protected virtual IImageSet CreateImageSet(MprVolume volume)
			{
				int number = 0;
				ImageSet imageSet = new ImageSet();
				foreach (IMprSliceSet sliceSet in volume.SliceSets)
				{
					imageSet.DisplaySets.Add(CreateDisplaySet(++number, sliceSet));
				}
				imageSet.Name = volume.Description;
				return imageSet;
			}

			protected override void BuildLogicalWorkspace()
			{
				foreach (MprVolume volume in this.ImageViewer.Volumes)
				{
					this.ImageViewer.MprWorkspace.ImageSets.Add(CreateImageSet(volume));
				}
			}

			protected override void LayoutPhysicalWorkspace()
			{
				base.ImageViewer.PhysicalWorkspace.SetImageBoxGrid(2, 2);

				foreach (IImageBox imageBox in base.ImageViewer.PhysicalWorkspace.ImageBoxes)
					imageBox.SetTileGrid(1, 1);

				base.ImageViewer.PhysicalWorkspace.Locked = true;
			}

			protected override void FillPhysicalWorkspace()
			{
				// Do our own filling. The base method clones the display set, which is:
				// 1. Time consuming, because of the header generation
				// 2. Useless, because it can never be shown, and the workspace is locked anyway so you don't need to "recover" the original
				// 3. Makes reslicing slow, since you generate two sets of presentation images
				// 4. All of the above

				IPhysicalWorkspace physicalWorkspace = ImageViewer.PhysicalWorkspace;
				IMprWorkspace mprWorkspace = ImageViewer.MprWorkspace;

				if (mprWorkspace.ImageSets.Count == 0)
					return;

				int imageSetIndex = 0;
				int displaySetIndex = 0;

				foreach (IImageBox imageBox in physicalWorkspace.ImageBoxes)
				{
					if (displaySetIndex == mprWorkspace.ImageSets[imageSetIndex].DisplaySets.Count)
					{
						imageSetIndex++;
						displaySetIndex = 0;

						if (imageSetIndex == mprWorkspace.ImageSets.Count)
							break;
					}

					imageBox.DisplaySet = mprWorkspace.ImageSets[imageSetIndex].DisplaySets[displaySetIndex];
					imageBox.DisplaySetLocked = true;
					displaySetIndex++;
				}

				// Let's start out in the middle of each stack
				foreach (IImageBox imageBox in this.ImageViewer.PhysicalWorkspace.ImageBoxes)
				{
					imageBox.TopLeftPresentationImageIndex = imageBox.DisplaySet.PresentationImages.Count/2;
				}
			}
		}

		#endregion
	}
}