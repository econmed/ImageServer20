﻿#region License

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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using Matrix = System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An <see cref="InvariantPrimitive"/> that can be described by a
	/// rectangular bounding box.
	/// </summary>
	public abstract class InvariantBoundablePrimitive : InvariantPrimitive, IBoundableGraphic
	{
		private RectangleF _rectangle = new RectangleF(0,0,0,0);

		/// <summary>
		/// Gets the top left corner of the rectangle in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF TopLeft
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					this.CoordinateSystem = CoordinateSystem.Destination;
					PointF topLeft = this.SpatialTransform.ConvertToSource(this.TopLeft);
					this.ResetCoordinateSystem();

					return topLeft;
				}
				else
				{
					return ConvertInvariantToDestination(InvariantTopLeft);
				}
			}
		}

		/// <summary>
		/// Gets or sets the top left corner of the rectangle in <i>screen</i> pixels relative to 
		/// the anchor point.
		/// </summary>
		/// <remarks>
		/// <see cref="InvariantTopLeft"/> is different from <see cref="TopLeft"/> in that
		/// it is the top left corner of the rectangle in screen pixels where (0,0)
		/// is the anchor point. For example, if you wanted an
		/// invariant square of size 9x9 screen pixels and the anchor point is in the middle
		/// of the square, <see cref="InvariantTopLeft"/> would be (-4,-4) and 
		/// <see cref="InvariantBottomRight"/> would be (4,4).
		/// </remarks>
		public PointF InvariantTopLeft
		{
			get { return _rectangle.Location; }
			set { _rectangle.Location = value; }
		}

		/// <summary>
		/// Gets the bottom right corner of the rectangle in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF BottomRight
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					this.CoordinateSystem = CoordinateSystem.Destination;
					PointF bottomRight = this.SpatialTransform.ConvertToSource(this.BottomRight);
					this.ResetCoordinateSystem();

					return bottomRight;
				}
				else
				{
					return ConvertInvariantToDestination(InvariantBottomRight);
				}
			}
		}

		/// <summary>
		/// Gets or sets the bottom right corner of the rectangle in <i>screen</i> pixels relative to 
		/// the anchor point.
		/// </summary>
		/// <remarks>
		/// <see cref="InvariantBottomRight"/> is different from <see cref="BottomRight"/> in that
		/// it is the bottom right corner of the rectangle in screen pixels where (0,0)
		/// is the anchor point. For example, if you wanted an
		/// invariant square of size 9x9 screen pixels and the anchor point is in the middle
		/// of the square, <see cref="InvariantTopLeft"/> would be (-4,-4) and 
		/// <see cref="InvariantBottomRight"/> would be (4,4).
		/// </remarks>
		public PointF InvariantBottomRight
		{
			get
			{
				return new PointF(_rectangle.X + _rectangle.Width, _rectangle.Y + _rectangle.Height);
			}
			set
			{
				_rectangle.Width = value.X - _rectangle.X;
				_rectangle.Height = value.Y - _rectangle.Y;
			}
		}

		/// <summary>
		/// Gets the width of the rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination pixels.
		/// </remarks>
		public float Width
		{
			get
			{
				return this.BottomRight.X - this.TopLeft.X;
			}
		}

		/// <summary>
		/// Gets the height of the rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public float Height
		{
			get
			{
				return this.BottomRight.Y - this.TopLeft.Y;
			}
		}

		/// <summary>
		/// Gets the rectangle that defines a <see cref="BoundableGraphic"/>.
		/// </summary>
		protected RectangleF Rectangle
		{
			get
			{
				return new RectangleF(this.TopLeft.X, this.TopLeft.Y, this.Width, this.Height);
			}
		}

		private PointF ConvertInvariantToDestination(PointF invariantPoint)
		{
			PointF xVector = new PointF(100, 0);
			SizeF xVectorTransformed = base.SpatialTransform.ConvertToDestination(new SizeF(xVector));

			//figure out where the source x-axis went in destination
			int rotation = (int)Math.Round(Vector.SubtendedAngle(xVectorTransformed.ToPointF(), PointF.Empty, xVector));
			if (rotation < 0)
				rotation += 360;

			Matrix m = new Matrix();
			m.Rotate(rotation);
			PointF[] pt = { invariantPoint };
			m.TransformPoints(pt);
			m.Dispose();

			return new PointF(base.AnchorPoint.X + pt[0].X, base.AnchorPoint.Y + pt[0].Y);
		}
	}
}