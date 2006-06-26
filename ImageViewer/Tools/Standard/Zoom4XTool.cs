using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	/// <summary>
	/// Summary description for ZoomTool.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.ImageWorkspaceToolExtensionPoint))]
	public class Zoom4XTool : ZoomFixedTool
	{
		public Zoom4XTool()
		{
		}

		private void GetDefaults()
		{
			//base.MenuPath.AddStandardItem(Model.SR.MenuTools);
			//base.MenuPath.AddStandardItem(Model.SR.MenuToolsStandard);
			//base.MenuPath.AddCustomItem(SR.MenuToolsStandardZoom, 30);

			//base.ToolbarPath.AddStandardItem(Model.SR.ToolbarStandard);
			//base.ToolbarPath.AddCustomItem(SR.ToolbarToolsStandardZoom, 50);
			//base.ToolbarPath.AddCustomItem(SR.ToolbarToolsStandardZoom4X, 30);

			//base.Tooltip = SR.ToolbarToolsStandardZoom4X;
		}

        public override void Activate()
        {
            this.ApplyZoom(4.0f);
        }
    }
}
