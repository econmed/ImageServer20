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

using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer {
	/// <summary>
	/// The interface for all DICOM-based Presentation Images.
	/// </summary>
	public interface IDicomPresentationImage :
		IPresentationImage,
		IImageSopProvider,
		IAnnotationLayoutProvider,
		IImageGraphicProvider,
		IApplicationGraphicsProvider,
		IOverlayGraphicsProvider,
		ISpatialTransformProvider,
		IPresentationStateProvider
		
	{
		/// <summary>
		/// Gets direct access to the presentation image's collection of domain-level graphics.
		/// Consider using <see cref="DicomGraphicsPlane.GetDicomGraphicsPlane(IDicomPresentationImage)"/> instead.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Use <see cref="DicomGraphics"/> to add DICOM-defined graphics that you want to
		/// overlay the image at the domain-level. These graphics are rendered
		/// before any <see cref="IApplicationGraphicsProvider.ApplicationGraphics"/>
		/// and before any <see cref="IOverlayGraphicsProvider.OverlayGraphics"/>.
		/// </para>
		/// <para>
		/// This property gives direct access to all the domain-level graphics of a DICOM presentation image.
		/// However, most of the graphics concepts defined in the DICOM Standard are already supported
		/// by the <see cref="DicomGraphicsPlane"/> which inserts itself into this domain-level collection.
		/// Consider using <see cref="DicomGraphicsPlane.GetDicomGraphicsPlane(IDicomPresentationImage)"/> to get
		/// a reference to a usable DicomGraphicsPlane object instead, since that provides all the logical support
		/// for layer activation and shutters in addition to enumerating all domain-level graphics. This property
		/// may change, be deprecated, and even outright removed in a future framework release.
		/// </para>
		/// </remarks>
		GraphicCollection DicomGraphics { get; }
	}
}
