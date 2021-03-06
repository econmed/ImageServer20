﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	partial class RotateObliqueTool
	{
		private class PinwheelGraphic : CompositeGraphic
		{
			private readonly InvariantLinePrimitive _line1;
			private readonly InvariantLinePrimitive _line2;
			private readonly InvariantEllipsePrimitive _ellipse;
			private PointF _anchor;

			public PinwheelGraphic()
			{
				base.Graphics.Add(_line1 = new InvariantLinePrimitive());
				base.Graphics.Add(_line2 = new InvariantLinePrimitive());
				base.Graphics.Add(_ellipse = new InvariantEllipsePrimitive());

				this.Color = Color.Yellow;
			}

			public Color Color
			{
				get { return _line1.Color; }
				set { _line1.Color = _line2.Color = value; }
			}

			public int Rotation
			{
				get { return base.SpatialTransform.RotationXY; }
				set { base.SpatialTransform.RotationXY = value; }
			}

			public PointF Anchor
			{
				get
				{
					if (base.CoordinateSystem == CoordinateSystem.Source)
					{
						return _anchor;
					}
					else
					{
						return base.SpatialTransform.ConvertToDestination(_anchor);
					}
				}
				private set
				{
					if (base.CoordinateSystem == CoordinateSystem.Source)
					{
						_anchor = value;
					}
					else
					{
						_anchor = base.SpatialTransform.ConvertToSource(value);
					}
				}
			}

			public override bool HitTest(Point point)
			{
				_ellipse.CoordinateSystem = CoordinateSystem.Destination;
				bool hit = _ellipse.Contains(point);
				_ellipse.ResetCoordinateSystem();
				return hit;
			}

			public PointF RotationAnchor
			{
				get { return _ellipse.Location; }
			}

			public override void OnDrawing()
			{
				base.CoordinateSystem = CoordinateSystem.Destination;

				int x = base.ParentPresentationImage.ClientRectangle.Width/2;
				int y = base.ParentPresentationImage.ClientRectangle.Height/2;
				this.Anchor = new PointF(x, y);

				base.ResetCoordinateSystem();

				_line1.Location = this.Anchor;
				_line1.InvariantTopLeft = new PointF(-7, 0);
				_line1.InvariantBottomRight = new PointF(20, 0);

				_line2.Location = this.Anchor;
				_line2.InvariantTopLeft = new PointF(0, -7);
				_line2.InvariantBottomRight = new PointF(0, 7);

				_ellipse.Location = _line1.BottomRight;
				_ellipse.InvariantTopLeft = new PointF(-3, -3);
				_ellipse.InvariantBottomRight = new PointF(3, 3);

				base.OnDrawing();
			}
		}
	}
}