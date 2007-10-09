using System;
using System.Diagnostics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class FocussedSelectedInteractiveGraphicState : FocussedSelectedGraphicState
	{
		public FocussedSelectedInteractiveGraphicState(InteractiveGraphic interactiveGraphic)
			: base(interactiveGraphic)
		{
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			InteractiveGraphic interactiveGraphic = base.StatefulGraphic as InteractiveGraphic;
			int controlPointIndex = interactiveGraphic.ControlPoints.HitTestControlPoint(mouseInformation.Location);

			// User has clicked a control point
			if (controlPointIndex != -1)
			{
				Trace.Write(String.Format("Control Point {0}\n", controlPointIndex.ToString()));
				base.StatefulGraphic.State = new MoveControlPointGraphicState(interactiveGraphic, controlPointIndex);
				base.StatefulGraphic.State.Start(mouseInformation);
				return true;
			}

			return base.Start(mouseInformation);
		}

		public override string ToString()
		{
			return "FocusSelectedInteractiveGraphicState\n";
		}
	}
}
