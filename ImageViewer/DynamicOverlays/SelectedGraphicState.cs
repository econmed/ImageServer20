using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class SelectedGraphicState : GraphicState
	{
		public SelectedGraphicState(StatefulGraphic graphic)
			: base(graphic)
		{
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			// In order to indicate which tool will be acted upon,
			// the selected graphic must also be able to be focused.
			if (base.StatefulGraphic.HitTest(e))
			{
				base.StatefulGraphic.State = base.StatefulGraphic.CreateFocusState();
				return true;
			}

			return false;
		}

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			bool hit = base.StatefulGraphic.HitTest(e);

			// User has clicked the graphic body
			if (hit)
			{
				base.StatefulGraphic.State = new MoveGraphicState(base.StatefulGraphic);
				base.StatefulGraphic.State.OnMouseDown(e);
				return true;
			}

			// User has clicked elsewhere in the image, so return false so
			// that the active modal tool can handle the click
			base.StatefulGraphic.State = base.StatefulGraphic.CreateInactiveState();
			return false;
		}

		public override void OnEnterState(XMouseEventArgs e)
		{
			base.StatefulGraphic.OnEnterSelectedState(e);
		}

		public override string ToString()
		{
			return "SelectedGraphicState\n";
		}
	}
}
