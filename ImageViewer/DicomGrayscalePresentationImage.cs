#region License

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

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A DICOM grayscale <see cref="PresentationImage"/>.
	/// </summary>
	public class DicomGrayscalePresentationImage 
		: GrayscalePresentationImage, IImageSopProvider
	{
		private readonly ImageSop _imageSop;
		private readonly Frame _frame;

		/// <summary>
		/// Initializes a new instance of <see cref="DicomGrayscalePresentationImage"/>.
		/// </summary>
		/// <param name="frame">The <see cref="Frame"/> from which to construct the image.</param>
		/// <remarks>
		/// This constructor provides a convenient means of associating a
		/// <see cref="Frame"/> with a <see cref="GrayscalePresentationImage"/>.
		/// </remarks>
		public DicomGrayscalePresentationImage(Frame frame)
			: base(frame.Rows,
				   frame.Columns,
				   frame.BitsAllocated,
				   frame.BitsStored,
				   frame.HighBit,
				   frame.PixelRepresentation != 0 ? true : false,
				   frame.PhotometricInterpretation == PhotometricInterpretation.Monochrome1 ? true : false,
				   frame.RescaleSlope,
				   frame.RescaleIntercept,
				   frame.PixelSpacing.Column,
				   frame.PixelSpacing.Row,
				   frame.PixelAspectRatio.Column,
				   frame.PixelAspectRatio.Row,
				   frame.GetNormalizedPixelData)
		{
			Platform.CheckForNullReference(frame, "frame");
			_frame = frame;
			_imageSop = frame.ParentImageSop;

			base.AnnotationLayoutProvider = new DicomFilteredAnnotationLayoutProvider(this);
		}

		/// <summary>
		/// Creates a fresh copy of the <see cref="DicomGrayscalePresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="DicomGrayscalePresentationImage"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		/// <returns></returns>		
		public override IPresentationImage CreateFreshCopy()
		{
			return new DicomGrayscalePresentationImage(_frame);
		}

		#region IImageSopProvider members

		/// <summary>
		/// Gets this presentation image's associated <see cref="ImageSop"/>.
		/// </summary>
		/// <remarks>
		/// Use <see cref="ImageSop"/> to access DICOM tags.
		/// </remarks>
		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		/// <summary>
		/// Gets this presentation image's associated <see cref="Frame"/>.
		/// </summary>
		public Frame Frame
		{
			get { return _frame; }
		}

		/// <summary>
		/// Returns the Instance Number as a string.
		/// </summary>
		/// <returns>The Instance Number as a string.</returns>
		public override string ToString()
		{
			return _imageSop.InstanceNumber.ToString();
		}

		#endregion
	}
}
