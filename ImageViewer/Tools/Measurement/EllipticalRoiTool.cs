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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuEllipticalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarEllipticalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.EllipticalRoiToolSmall.png", "Icons.EllipticalRoiToolMedium.png", "Icons.EllipticalRoiToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Measurement.Roi.Elliptical")]

	[MouseToolButton(XMouseButtons.Left, false)]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class EllipticalRoiTool : MeasurementTool
	{
		public EllipticalRoiTool()
			: base(SR.TooltipEllipticalRoi)
		{
		}

		protected override InteractiveGraphic  CreateInteractiveGraphic()
		{
			return new EllipseInteractiveGraphic(true);
		}

		protected override string CreationCommandName
		{
			get { return SR.CommandCreateEllipiticalRoi; }
		}

		protected override void OnRoiCreation(RoiGraphic roiGraphic)
		{
			roiGraphic.Roi.ControlPoints.Visible = false;
		}

		protected override object[] CreateAnalyzers()
		{
			EllipseAnalyzerExtensionPoint extensionPoint = new EllipseAnalyzerExtensionPoint();
			return extensionPoint.CreateExtensions();
		}
	}

	public interface IEllipseAnalyzer : IRoiAnalyzer
	{
		string Analyze(ImageSop sop, EllipseInteractiveGraphic rectangle);
	}

	[ExtensionPoint]
	public sealed class EllipseAnalyzerExtensionPoint : ExtensionPoint<IEllipseAnalyzer>
	{
	}
}