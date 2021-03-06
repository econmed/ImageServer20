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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A <see cref="VectorGraphic"/> whose size in destination coordinates is invariant
	/// under zoom.
	/// </summary>
	/// <remarks>
	/// Sometimes it is desirable to have a <see cref="VectorGraphic"/> whose
	/// <i>size</i> is invariant under zoom, but whose position is not.  A good example
	/// are the <see cref="ControlPoint"/> objects on measurement tools that allow a 
	/// user to stretch and resize the measurement.  They are anchored to a certain
	/// point on the underlying image so that when zoomed, the control point appears
	/// to "move" with the zoom of the image, but their size
	/// in screen pixels remains the same.
	/// </remarks>
	[Cloneable(true)]
	public abstract class InvariantPrimitive : VectorGraphic
	{
		private PointF _location;
		private event EventHandler<PointChangedEventArgs> _locationChangedEvent;

		/// <summary>
		/// Initializes a new instance of <see cref="InvariantPrimitive"/>.
		/// </summary>
		protected InvariantPrimitive()
		{
		}

		/// <summary>
		/// The location where the <see cref="InvariantPrimitive"/> is anchored.
		/// </summary>
		/// <remarks>
		/// The value of this property is in either source or destination coordinates
		/// depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		public PointF Location
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return _location;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_location);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.Location, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					_location = value;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_location = base.SpatialTransform.ConvertToSource(value);
				}

				this.OnLocationChanged();
				base.NotifyVisualStateChanged("Location");
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Location"/> property has changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> LocationChanged
		{
			add { _locationChangedEvent += value; }
			remove { _locationChangedEvent -= value; }
		}

		/// <summary>
		/// Called when the value of <see cref="Location"/> changes.
		/// </summary>
		protected virtual void OnLocationChanged()
		{
			EventsHelper.Fire(_locationChangedEvent, this, new PointChangedEventArgs(this.Location));
		}

		/// <summary>
		/// Moves the <see cref="Graphic"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public override void Move(SizeF delta)
		{
#if MONO
			Size del = new Size((int)delta.Width, (int)delta.Height);
			this.Location += del;
#else
			this.Location += delta;
#endif
		}

		#region Legacy Members

		/// <summary>
		/// Occurs when the <see cref="Location"/> property has changed.
		/// </summary>
		[Obsolete("Use the LocationChanged event instead.")]
		public event EventHandler<PointChangedEventArgs> AnchorPointChanged
		{
			add { _locationChangedEvent += value; }
			remove { _locationChangedEvent -= value; }
		}

		/// <summary>
		/// The point in the coordinate system of the parent <see cref="IGraphic"/> where the
		/// <see cref="InvariantPrimitive"/> is anchored.
		/// </summary>
		[Obsolete("Use the Location property instead.")]
		public PointF AnchorPoint
		{
			get { return this.Location; }
			set { this.Location = value; }
		}

		#endregion
	}
}
