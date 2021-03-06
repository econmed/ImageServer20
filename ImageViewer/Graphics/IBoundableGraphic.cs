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

using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines an <see cref="IVectorGraphic"/> that can be described by a
	/// rectangular bounding box.
	/// </summary>
	/// <remarks>
	/// Rectangles and ellipses are examples of graphics that can be
	/// described by a rectangular bounding box.
	/// </remarks>
	public interface IBoundableGraphic : IVectorGraphic 
	{
		/// <summary>
		/// Occurs when the <see cref="TopLeft"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> TopLeftChanged;

		/// <summary>
		/// Occurs when the <see cref="BottomRight"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> BottomRightChanged;

		/// <summary>
		/// Gets or sets the top left corner of the bounding rectangle in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		PointF TopLeft { get; set; }

		/// <summary>
		/// Gets or sets the bottom right corner of the bounding rectangle in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		PointF BottomRight { get; set; }

		/// <summary>
		/// Gets the bounding rectangle of the graphic in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property gives the orientation-sensitive rectangle that bounds the graphic, whereas
		/// the <see cref="IGraphic.BoundingBox"/> property gives the normalized rectangle with positive
		/// width and height.
		/// </para>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		/// <seealso cref="IGraphic.BoundingBox"/>
		RectangleF Rectangle { get; }

		/// <summary>
		/// Gets the width of the bounding rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		float Width { get; }

		/// <summary>
		/// Gets the height of the bounding rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		float Height { get; }

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		bool Contains(PointF point);
	}
}