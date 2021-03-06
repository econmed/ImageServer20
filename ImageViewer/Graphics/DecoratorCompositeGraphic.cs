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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.PresentationStates.Dicom.GraphicAnnotationSerializers;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines <see cref="IGraphic"/>s which follow the <i>decorator pattern</i> to provide
	/// modify and/or add functionality to an existing <see cref="IGraphic"/>.
	/// </summary>
	public interface IDecoratorGraphic : IGraphic
	{
		/// <summary>
		/// Gets the <see cref="IGraphic"/> decorated by this graphic.
		/// </summary>
		IGraphic DecoratedGraphic { get; }
	}

	/// <summary>
	/// Base class for <see cref="IDecoratorGraphic"/> implementations.
	/// </summary>
	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof (DecoratorGraphicAnnotationSerializer))]
	public abstract class DecoratorCompositeGraphic : CompositeGraphic, IDecoratorGraphic
	{
		[CloneIgnore]
		private IGraphic _decoratedGraphic;

		/// <summary>
		/// Constructs a new <see cref="DecoratorCompositeGraphic"/>.
		/// </summary>
		/// <param name="graphic">The graphic to be decorated.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="graphic"/> is null.</exception>
		protected DecoratorCompositeGraphic(IGraphic graphic)
		{
			Platform.CheckForNullReference(graphic, "graphic");
			base.Graphics.Add(_decoratedGraphic = graphic);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected DecoratorCompositeGraphic(DecoratorCompositeGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_decoratedGraphic = CollectionUtils.FirstElement(base.Graphics);
		}

		/// <summary>
		/// Gets the <see cref="IGraphic"/> decorated by this graphic.
		/// </summary>
		IGraphic IDecoratorGraphic.DecoratedGraphic
		{
			get { return this.DecoratedGraphic; }
		}

		/// <summary>
		/// Gets the <see cref="IGraphic"/> decorated by this graphic.
		/// </summary>
		protected IGraphic DecoratedGraphic
		{
			get { return _decoratedGraphic; }
		}

		/// <summary>
		/// Gets an object describing the region of interest on the <see cref="Graphic.ParentPresentationImage"/> selected by the <see cref="DecoratedGraphic"/>.
		/// </summary>
		/// <remarks>
		/// Decorated graphics that do not describe a region of interest may return null.
		/// </remarks>
		/// <returns>A <see cref="Roi"/> describing this region of interest, or null if the decorated graphic does not describe a region of interest.</returns>
		public override Roi GetRoi()
		{
			return this.DecoratedGraphic.GetRoi();
		}
	}
}