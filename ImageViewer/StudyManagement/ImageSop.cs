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

using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM Image SOP Instance.
	/// </summary>
	public abstract class ImageSop : Sop
	{
		private FrameCollection _frames;

		/// <summary>
		/// Initializes a new instance of <see cref="ImageSop"/>.
		/// </summary>
		protected ImageSop()
		{
		}

		/// <summary>
		/// A collection of <see cref="Frame"/> objects.
		/// </summary>
		/// <remarks>
		/// DICOM distinguishes between regular image SOPs and multiframe image SOPs.
		/// ClearCanvas, however, does not make this distinction, as it requires that 
		/// two sets of client code be written.  Instead, all image SOPs are considered
		/// to be multiframe, with regular images being a special case of a multiframe
		/// image with one frame. It can be assumed that all images contain at least
		/// one frame.
		/// </remarks>
		/// <seealso cref="NumberOfFrames"/>
		public virtual FrameCollection Frames
		{
			get
			{
				if (_frames == null)
				{
					_frames = new FrameCollection();
					AddFrames();
				}

				return _frames;
			}
		}

		#region General Image Module

		/// <summary>
		/// Gets the patient orientation.
		/// </summary>
		/// <remarks>
		/// A <see cref="PatientOrientation"/> is returned even when no data is available; 
		/// it will simply have values of "" for its 
		/// <see cref="ClearCanvas.Dicom.PatientOrientation.Row"/> and 
		/// <see cref="ClearCanvas.Dicom.PatientOrientation.Column"/> properties.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual PatientOrientation PatientOrientation
		{
			get { return this.Frames[1].PatientOrientation; }
		}

		/// <summary>
		/// Gets the image type.  The entire Image Type value should be returned as a Dicom string array.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string ImageType
		{
			get { return this.Frames[1].ImageType; }
		}

		/// <summary>
		/// Gets the acquisition number.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int AcquisitionNumber
		{
			get { return this.Frames[1].AcquisitionNumber; }
		}

		/// <summary>
		/// Gets the acquisiton date.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string AcquisitionDate
		{
			get { return this.Frames[1].AcquisitionDate; }
		}

		/// <summary>
		/// Gets the acquisition time.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string AcquisitionTime
		{
			get { return this.Frames[1].AcquisitionTime; }
		}

		/// <summary>
		/// Gets the acquisition date/time.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string AcquisitionDateTime
		{
			get { return this.Frames[1].AcquisitionDateTime; }
		}

		/// <summary>
		/// Gets the number of images in the acquisition.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int ImagesInAcquisition
		{
			get { return this.Frames[1].ImagesInAcquisition; }
		}

		/// <summary>
		/// Gets the image comments.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string ImageComments
		{
			get { return this.Frames[1].ImageComments; }
		}

		/// <summary>
		/// Gets the lossy image compression.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string LossyImageCompression
		{
			get { return this.Frames[1].LossyImageCompression; }
		}

		/// <summary>
		/// Gets the lossy image compression ratio.
		/// </summary>
		/// <remarks>
		/// Will return as many parsable values as possible up to the first non-parsable value.  For example, if there are 3 values, but the last one is poorly encoded, 2 values will be returned.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual double[] LossyImageCompressionRatio
		{
			get { return this.Frames[1].LossyImageCompressionRatio; }
		}

		#endregion

		#region Image Plane Module

		/// <summary>
		/// Gets the pixel spacing.
		/// </summary>
		/// <remarks>
		/// It is generally recommended that clients use <see cref="NormalizedPixelSpacing"/> when
		/// in calculations that require the pixel spacing.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual PixelSpacing PixelSpacing
		{
			get { return this.Frames[1].PixelSpacing; }
		}

		/// <summary>
		/// Gets the image orientation patient.
		/// </summary>
		/// <remarks>
		/// Returns an <see cref="ImageOrientationPatient"/> object with zero for all its values when no data is available or the existing data is bad/incorrect.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual ImageOrientationPatient ImageOrientationPatient
		{
			get { return this.Frames[1].ImageOrientationPatient; }
		}

		/// <summary>
		/// Gets the image position patient.
		/// </summary>
		/// <remarks>
		/// Returns an <see cref="ImagePositionPatient"/> object with zero for all its values when no data is available or the existing data is bad/incorrect.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual ImagePositionPatient ImagePositionPatient
		{
			get { return this.Frames[1].ImagePositionPatient; }
		}

		/// <summary>
		/// Gets the slice thickness.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual double SliceThickness
		{
			get { return this.Frames[1].SliceThickness; }
		}

		/// <summary>
		/// Gets the slice location.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual double SliceLocation
		{
			get { return this.Frames[1].SliceLocation; }
		}

		#endregion

		#region Image Pixel Module

		#region Type 1
		
		/// <summary>
		/// Gets the samples per pixel.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int SamplesPerPixel
		{
			get { return this.Frames[1].SamplesPerPixel; }
		}

		/// <summary>
		/// Gets the photometric interpretation.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual PhotometricInterpretation PhotometricInterpretation
		{
			get { return this.Frames[1].PhotometricInterpretation; }
		}

		/// <summary>
		/// Gets the number of rows.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int Rows
		{
			get { return this.Frames[1].Rows; }
		}

		/// <summary>
		/// Gets the number of columns.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int Columns
		{
			get { return this.Frames[1].Columns; }
		}

		/// <summary>
		/// Gets the number of bits allocated.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int BitsAllocated
		{
			get { return this.Frames[1].BitsAllocated; }
		}

		/// <summary>
		/// Gets the number of bits stored.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int BitsStored
		{
			get { return this.Frames[1].BitsStored; }
		}

		/// <summary>
		/// Gets the high bit.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int HighBit
		{
			get { return this.Frames[1].HighBit; }
		}

		/// <summary>
		/// Gets the pixel representation.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int PixelRepresentation
		{
			get { return this.Frames[1].PixelRepresentation; }
		}

		#endregion 
		#region Type 1C

		/// <summary>
		/// Gets the planar configuration.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual int PlanarConfiguration
		{
			get { return this.Frames[1].PlanarConfiguration; }
		}

		/// <summary>
		/// Gets the pixel aspect ratio.
		/// </summary>
		/// <remarks>
		/// A default value of 1/1 is returned if no data is available.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual PixelAspectRatio PixelAspectRatio
		{
			get { return this.Frames[1].PixelAspectRatio; }
		}

		#endregion
		#endregion

		#region Modality LUT Module

		/// <summary>
		/// Gets the rescale intercept.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual double RescaleIntercept
		{
			get { return this.Frames[1].RescaleIntercept; }
		}

		/// <summary>
		/// Gets the rescale slope.
		/// </summary>
		/// <remarks>
		/// 1.0 is returned if no data is available.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual double RescaleSlope
		{
			get { return this.Frames[1].RescaleSlope; }
		}

		/// <summary>
		/// Gets the rescale type.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string RescaleType
		{
			get { return this.Frames[1].RescaleType; }
		}

		#endregion

		#region VOI LUT Module

		/// <summary>
		/// Gets the window width and center.
		/// </summary>
		/// <remarks>
		/// Will return as many parsable values as possible up to the first non-parsable value.  For example, if there are 3 values, but the last one is poorly encoded, 2 values will be returned.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual Window[] WindowCenterAndWidth
		{
			get { return this.Frames[1].WindowCenterAndWidth; }
		}

		/// <summary>
		/// Gets the window width and center explanation.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string[] WindowCenterAndWidthExplanation
		{
			get { return this.Frames[1].WindowCenterAndWidthExplanation; }
		}

		#endregion

		#region Frame of Reference Module

		/// <summary>
		/// Gets the frame of reference uid for the image.
		/// </summary>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual string FrameOfReferenceUid
		{
			get { return this.Frames[1].FrameOfReferenceUid; }
		}

		#endregion

		#region Multiframe Module

		/// <summary>
		/// Gets the number of frames in the image SOP.
		/// </summary>
		/// <remarks>
		/// Regular, non-multiframe DICOM images do not have this tag. However, because 
		/// such images are treated as multiframes with a single frame, 
		/// <see cref="NumberOfFrames"/> returns 1 in that case.
		/// </remarks>
		public virtual int NumberOfFrames
		{
			get
			{
				bool tagExists;
				int numberOfFrames;
				GetTag(DicomTags.NumberOfFrames, out numberOfFrames, out tagExists);

				return Math.Max(numberOfFrames, 1);
			}
		}

		#endregion

		/// <summary>
		/// Gets pixel data in normalized form.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// <i>Normalized</i> pixel data means that:
		/// <list type="Bullet">
		/// <item>
		/// <description>Grayscale pixel data is unchanged.</description>
		/// </item>
		/// <item>
		/// <description>Colour pixel data is always converted
		/// into ARGB format.</description>
		/// </item>
		/// <item>
		/// <description>Pixel data is always uncompressed.</description>
		/// </item>
		/// </list>
		/// Ensuring that the pixel data always meets the above criteria
		/// allows clients to easily consume pixel data without having
		/// to worry about the the multitude of DICOM photometric interpretations
		/// and transfer syntaxes.
		/// </remarks>
		[Obsolete("This method has been deprecated and will be removed in the future. Use equivalent property on Frame class instead.")]
		public virtual byte[] GetNormalizedPixelData()
		{
			return this.Frames[1].GetNormalizedPixelData();
		}

		/// <summary>
		/// Adds <see cref="Frame"/> objects to <see cref="ImageSop.Frames"/>.
		/// </summary>
		/// <remarks>
		/// It is the responsibility of the subclass to add the frames to the collection.
		/// </remarks>
		protected abstract void AddFrames();

		/// <summary>
		/// Validates the <see cref="ImageSop"/> object.
		/// </summary>
		/// <remarks>
		/// Derived classes should call the base class implementation first, and then do further validation.
		/// The <see cref="ImageSop"/> class validates properties deemed vital to usage of the object.
		/// </remarks>
		/// <exception cref="SopValidationException">Thrown when validation fails.</exception>
		protected override void ValidateInternal()
		{
			base.ValidateInternal();

			ValidateAllowableTransferSyntax();

			foreach (Frame frame in this.Frames)
			{
				frame.Validate();
			}
		}

		private void ValidateAllowableTransferSyntax()
		{
			//Right now, Dicom Images are restricted to these transfer syntaxes for viewing purposes.
			if (this.TransferSyntaxUID != "1.2.840.10008.1.2" &&
				this.TransferSyntaxUID != "1.2.840.10008.1.2.1" &&
				this.TransferSyntaxUID != "1.2.840.10008.1.2.2" &&
				this.TransferSyntaxUID != "1.2.840.10008.1.2.5")
				throw new SopValidationException(SR.ExceptionInvalidTransferSyntaxUID);
		}
	}
}
