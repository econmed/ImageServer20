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
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An interactive polyline graphic.
	/// </summary>
	public class PolyLineInteractiveGraphic : InteractiveGraphic
	{
		#region Private fields

		private PolyLineGraphic _anchorPointsGraphic = new PolyLineGraphic();
		private int _maxAnchorPoints;
		private CursorToken _moveToken;
		private Color _color = Color.Yellow;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="PolyLineInteractiveGraphic"/>
		/// </summary>
		/// <param name="userCreated">Indicates whether the graphic was created
		/// through user interaction.</param>
		/// <param name="maximumAnchorPoints">The maximum number of points in
		/// the <see cref="PolyLineInteractiveGraphic"/>.
		/// </param>
		public PolyLineInteractiveGraphic(bool userCreated, int maximumAnchorPoints)
			: base(userCreated)
		{
			_maxAnchorPoints = maximumAnchorPoints;
			BuildGraphic();
			_moveToken = new CursorToken(CursorToken.SystemCursors.SizeAll);
		}

		/// <summary>
		/// Gets the maximum number of anchor points in the polyline.
		/// </summary>
		public int MaximumAnchorPoints
		{
			get { return _maxAnchorPoints; }
		}

		/// <summary>
		/// Gets the <see cref="PolyLineGraphic"/>.
		/// </summary>
		public PolyLineGraphic PolyLine
		{
			get { return _anchorPointsGraphic; }
		}

		/// <summary>
		/// Gets or sets the colour of the <see cref="PolyLineInteractiveGraphic"/>.
		/// </summary>
		public override Color Color
		{
			get { return base.Color; }
			set
			{
				_anchorPointsGraphic.Color = value;
				base.Color = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="CursorToken"/> that should be shown
		/// when moving this graphic.
		/// </summary>
		public CursorToken MoveToken
		{
			get { return _moveToken; }
			set { _moveToken = value; }
		}

		#region IMemorable Members

		/// <summary>
		/// Captures the state of the <see cref="PolyLineInteractiveGraphic"/>.
		/// </summary>
		/// <returns></returns>
		public override object CreateMemento()
		{
			return this.PolyLine.CreateMemento();
		}

		/// <summary>
		/// Restores the state of the <see cref="PolyLineInteractiveGraphic"/>.
		/// </summary>
		/// <param name="memento"></param>
		public override void SetMemento(object memento)
		{
			this.PolyLine.SetMemento(memento);
		}

		#endregion

		/// <summary>
		/// Moves the <see cref="PolyLineInteractiveGraphic"/> by
		/// the specified delta.
		/// </summary>
		/// <param name="delta"></param>
		public override void Move(SizeF delta)
		{
#if MONO
			Size del = new Size((int)delta.Width, (int)delta.Height);
			
			for (int i = 0; i < this.AnchorPoints.Count; i++)
				this.AnchorPoints[i] += del;
#else
			for (int i = 0; i < this.PolyLine.Count; i++)
				this.PolyLine[i] += delta;
#endif
		}

		/// <summary>
		/// Creates a creation <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public override GraphicState CreateCreateState()
		{
			return new CreatePolyLineGraphicState(this);
		}

		/// <summary>
		/// Performs a hit test on the <see cref="PolyLineInteractiveGraphic"/>.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool HitTest(Point point)
		{
			return this.PolyLine.HitTest(point);
		}

		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Executed when the position of an anchor point has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnAnchorPointChanged(object sender, CollectionEventArgs<PointF> e)
		{
			// This acts as a mediator.  It listens for changes in the anchor points
			// and make corresponding changes in the position of the control points.
			base.ControlPoints[e.Index] = e.Item;
			Trace.Write(String.Format("OnAnchorPointChanged: {0}, {1}\n", e.Index, e.Item.ToString()));
		}

		/// <summary>
		/// Executed when a the position of a control point has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnControlPointChanged(object sender, CollectionEventArgs<PointF> e)
		{
			this.PolyLine[e.Index] = e.Item;
			Trace.Write(String.Format("OnControlPointChanged: {0}, {1}\n", e.Index, e.Item.ToString()));
		}

		/// <summary>
		/// Releases all resources used by this <see cref="PolyLineInteractiveGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				_anchorPointsGraphic.AnchorPointChangedEvent -= new EventHandler<CollectionEventArgs<PointF>>(OnAnchorPointChanged);

			base.Dispose(disposing);
		}

		private void BuildGraphic()
		{
			base.Graphics.Add(_anchorPointsGraphic);
			_anchorPointsGraphic.AnchorPointChangedEvent += new EventHandler<CollectionEventArgs<PointF>>(OnAnchorPointChanged);

			// Add two points to begin with
			this.PolyLine.Add(new PointF(0, 0));
			base.ControlPoints.Add(new PointF(0, 0));
			this.PolyLine.Add(new PointF(0, 0));
			base.ControlPoints.Add(new PointF(0, 0));
		}
	}
}
