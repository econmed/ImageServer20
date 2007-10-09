using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class PolyLineInteractiveGraphic : InteractiveGraphic
	{
		private PolyLineAnchorPointsGraphic _anchorPointsGraphic = new PolyLineAnchorPointsGraphic();
		private int _maxAnchorPoints;
		private CursorToken _moveToken;
		private Color _color = Color.Yellow;

		public PolyLineInteractiveGraphic(bool userCreated, int numberOfPoints)
			: base(userCreated)
		{
			_maxAnchorPoints = numberOfPoints;
			BuildGraphic();
		}

		public int MaximumAnchorPoints
		{
			get { return _maxAnchorPoints; }
			protected set { _maxAnchorPoints = value; }
		}

		public PolyLineAnchorPointsGraphic AnchorPoints
		{
			get { return _anchorPointsGraphic; }
		}

		public CursorToken MoveToken
		{
			get { return _moveToken; }
			set { _moveToken = value; }
		}

		public override Color Color
		{
			get { return base.Color; }
			set
			{
				_anchorPointsGraphic.Color = value;
				base.Color = value;
			}
		}

		#region IMemorable Members

		public override IMemento CreateMemento()
		{
			return this.AnchorPoints.CreateMemento();
		}

		public override void SetMemento(IMemento memento)
		{
			this.AnchorPoints.SetMemento(memento);
		}

		#endregion

		public override void Move(SizeF delta)
		{
#if MONO
			Size del = new Size((int)delta.Width, (int)delta.Height);
			
			for (int i = 0; i < this.AnchorPoints.Count; i++)
				this.AnchorPoints[i] += del;
#else
			for (int i = 0; i < this.AnchorPoints.Count; i++)
				this.AnchorPoints[i] += delta;
#endif
		}

		public override GraphicState CreateCreateState()
		{
			return new CreatePolyLineGraphicState(this);
		}

		public override bool HitTest(Point point)
		{
			return this.AnchorPoints.HitTest(point);
		}

		public override CursorToken GetCursorToken(Point point)
		{
			CursorToken token = base.GetCursorToken(point);
			if (token == null)
			{
				if (this.HitTest(point))
				{
					token = this.MoveToken;
				}
			}

			return token;
		}

		public override void InstallDefaultCursors()
		{
			base.InstallDefaultCursors();
			this.MoveToken = new CursorToken(CursorToken.SystemCursors.SizeAll);
		}

		// This acts as a mediator.  It listens for changes in the anchor points
		// and make corresponding changes in the position of the control points.
		protected void OnAnchorPointChanged(object sender, AnchorPointEventArgs e)
		{
			base.ControlPoints[e.AnchorPointIndex] = e.AnchorPoint;
			Trace.Write(String.Format("OnAnchorPointChanged: {0}, {1}\n", e.AnchorPointIndex, e.AnchorPoint.ToString()));
		}

		protected override void OnControlPointChanged(object sender, ControlPointEventArgs e)
		{
			this.AnchorPoints[e.ControlPointIndex] = e.ControlPointLocation;
			Trace.Write(String.Format("OnControlPointChanged: {0}, {1}\n", e.ControlPointIndex, e.ControlPointLocation.ToString()));
		}

		private void BuildGraphic()
		{
			base.Graphics.Add(_anchorPointsGraphic);
			_anchorPointsGraphic.AnchorPointChangedEvent += new EventHandler<AnchorPointEventArgs>(OnAnchorPointChanged);

			// Add two points to begin with
			this.AnchorPoints.Add(new PointF(0, 0));
			base.ControlPoints.Add(new PointF(0, 0));
			this.AnchorPoints.Add(new PointF(0, 0));
			base.ControlPoints.Add(new PointF(0, 0));
		}
	}
}
