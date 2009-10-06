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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public interface ISliceSopDataSource : ISopDataSource {}

	public class VolumeSliceSopDataSource : StandardSopDataSource, ISliceSopDataSource
	{
		private readonly IVolumeReference _volume;
		private readonly IVolumeSlicerParams _slicerParams;
		private readonly Matrix _resliceMatrix;
		private readonly DicomAttributeCollection _instanceDataSet;
		private readonly IList<Vector3D> _throughPoints;

		public VolumeSliceSopDataSource(Volume volume, IVolumeSlicerParams slicerParams, Vector3D throughPoint)
			: this(volume, slicerParams, new Vector3D[] {throughPoint}) {}

		public VolumeSliceSopDataSource(Volume volume, IVolumeSlicerParams slicerParams, IList<Vector3D> throughPoints)
		{
			Platform.CheckForNullReference(throughPoints, "throughPoints");
			Platform.CheckTrue(throughPoints.Count > 0, "At least one through point must be specified.");

			_volume = volume.CreateTransientReference();
			_slicerParams = slicerParams;
			_resliceMatrix = new Matrix(slicerParams.SlicingPlaneRotation);
			_resliceMatrix[3, 0] = throughPoints[0].X;
			_resliceMatrix[3, 1] = throughPoints[0].Y;
			_resliceMatrix[3, 2] = throughPoints[0].Z;
			_throughPoints = new List<Vector3D>(throughPoints).AsReadOnly();

			_instanceDataSet = new DicomAttributeCollection();

			// JY: ideally, each slicing plane is represented by a single multiframe SOP where the individual slices are the frames.
			// We need to support multi-valued Slice Location in the base viewer first.
			// When that is implemented, the SOPs should be created on the first frame of the slicing (i.e. one of the end slices)
			// and the Slice Location Vector will simply store the slice locations relative to that defined in these attributes.
			// Also, the rows and columns will have to be computed to be the MAX possible size (all frames must have same size)

			// assign Rows and Columns to reflect actual output size
			Size frameSize = GetSliceExtent(volume, slicerParams);
			_instanceDataSet[DicomTags.Columns].SetInt32(0, frameSize.Width);
			_instanceDataSet[DicomTags.Rows].SetInt32(0, frameSize.Height);

			// assign Image Orientation (Patient)
			Matrix resliceAxesPatientOrientation = _volume.Volume.RotateToPatientOrientation(_resliceMatrix);
			_instanceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(0, resliceAxesPatientOrientation[0, 0]);
			_instanceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(1, resliceAxesPatientOrientation[0, 1]);
			_instanceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(2, resliceAxesPatientOrientation[0, 2]);
			_instanceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(3, resliceAxesPatientOrientation[1, 0]);
			_instanceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(4, resliceAxesPatientOrientation[1, 1]);
			_instanceDataSet[DicomTags.ImageOrientationPatient].SetFloat32(5, resliceAxesPatientOrientation[1, 2]);

			// assign Image Position (Patient)
			Vector3D topLeftOfSlicePatient = GetTopLeftOfSlicePatient(frameSize, throughPoints[0], volume, slicerParams);
			_instanceDataSet[DicomTags.ImagePositionPatient].SetFloat32(0, topLeftOfSlicePatient.X);
			_instanceDataSet[DicomTags.ImagePositionPatient].SetFloat32(1, topLeftOfSlicePatient.Y);
			_instanceDataSet[DicomTags.ImagePositionPatient].SetFloat32(2, topLeftOfSlicePatient.Z);

			// assign Number of Frames
			_instanceDataSet[DicomTags.NumberOfFrames].SetInt32(0, throughPoints.Count);

			// assign a new SOP instance UID
			_instanceDataSet[DicomTags.SopInstanceUid].SetString(0, DicomUid.GenerateUid().UID);
		}

		public Volume Volume
		{
			get { return _volume.Volume; }
		}

		public IVolumeSlicerParams SlicerParams
		{
			get { return _slicerParams; }
		}

		public override DicomAttribute this[DicomTag tag]
		{
			get
			{
				DicomAttribute attribute;
				if (_volume.Volume.DataSet.TryGetAttribute(tag, out attribute))
					return attribute;
				return _instanceDataSet[tag];
			}
		}

		public override DicomAttribute this[uint tag]
		{
			get
			{
				DicomAttribute attribute;
				if (_volume.Volume.DataSet.TryGetAttribute(tag, out attribute))
					return attribute;
				return _instanceDataSet[tag];
			}
		}

		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			if (_volume.Volume.DataSet.TryGetAttribute(tag, out attribute))
				return true;
			return _instanceDataSet.TryGetAttribute(tag, out attribute);
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			if (_volume.Volume.DataSet.TryGetAttribute(tag, out attribute))
				return true;
			return _instanceDataSet.TryGetAttribute(tag, out attribute);
		}

		protected override StandardSopFrameData CreateFrameData(int frameNumber)
		{
			Platform.CheckArgumentRange(frameNumber, 1, _throughPoints.Count, "frameNumber");
			return new VolumeSliceSopFrameData(frameNumber, this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_volume.Dispose();
			}
			base.Dispose(disposing);
		}

		protected class VolumeSliceSopFrameData : StandardSopFrameData
		{
			public VolumeSliceSopFrameData(int frameNumber, VolumeSliceSopDataSource parent) : base(frameNumber, parent) {}

			public new VolumeSliceSopDataSource Parent
			{
				get { return (VolumeSliceSopDataSource) base.Parent; }
			}

			public Vector3D ThroughPoint
			{
				get { return this.Parent._throughPoints[base.FrameNumber - 1]; }
			}

			protected override byte[] CreateNormalizedPixelData()
			{
				using (VolumeSlicer slicer = new VolumeSlicer(this.Parent.Volume, this.Parent.SlicerParams, this.Parent.SeriesInstanceUid))
				{
					return slicer.CreateSliceNormalizedPixelData(this.ThroughPoint);
				}
			}

			protected override byte[] CreateNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber)
			{
				Debug.Assert(false, "We should never get here... we don't support overlays in the volume (yet)!!!");
				return new byte[0];
			}
		}

		#region Slcing

		#endregion

		#region Misc Helpers for computing SOP attribute values, originally in VolumeSlicer.cs

		// VTK treats the reslice point as the center of the output image. Given the plane orientation
		//	and size of the output image, we can derive the top left of the output image in patient space
		private static Vector3D GetTopLeftOfSlicePatient(Size frameSize, Vector3D throughPoint, Volume volume, IVolumeSlicerParams slicerParams)
		{
			// This is the center of the output image
			PointF centerImageCoord = new PointF(frameSize.Width/2f, frameSize.Height/2f);

			// These offsets define the x and y vector magnitudes to arrive at our point
			float effectiveSpacing = GetEffectiveSpacing(volume);
			float offsetX = centerImageCoord.X*effectiveSpacing;
			float offsetY = centerImageCoord.Y*effectiveSpacing;

			// To determine top left of slice in volume, subtract offset vectors along x and y
			//
			// Our reslice place x and y vectors
			Matrix resliceAxes = slicerParams.SlicingPlaneRotation;
			Vector3D xVec = new Vector3D(resliceAxes[0, 0], resliceAxes[0, 1], resliceAxes[0, 2]);
			Vector3D yVec = new Vector3D(resliceAxes[1, 0], resliceAxes[1, 1], resliceAxes[1, 2]);
			// Offset along x and y from reslicePoint
			Vector3D topLeftOfSliceVolume = throughPoint - (offsetX*xVec + offsetY*yVec);

			// Convert volume point to patient space
			return volume.ConvertToPatient(topLeftOfSliceVolume);
		}

		// Derived frome either a specified extent in millimeters or from the volume dimensions (default)
		private static Size GetSliceExtent(Volume volume, IVolumeSlicerParams slicerParams)
		{
			int rows, columns;

			float effectiveSpacing = GetEffectiveSpacing(volume);
			float longOutputDimension = volume.LongAxisMagnitude/effectiveSpacing;
			float shortOutputDimenstion = volume.ShortAxisMagnitude/effectiveSpacing;
			rows = columns = (int) Math.Sqrt(longOutputDimension*longOutputDimension + shortOutputDimenstion*shortOutputDimenstion);

			if (slicerParams.SliceExtentXMillimeters != 0f)
				columns = (int) (slicerParams.SliceExtentXMillimeters/effectiveSpacing + 0.5f);

			if (slicerParams.SliceExtentYMillimeters != 0f)
				rows = (int) (slicerParams.SliceExtentYMillimeters/effectiveSpacing + 0.5f);

			return new Size(columns, rows);
		}

		/// <summary>
		/// The effective spacing defines output spacing for slices generated by the VolumeSlicer.
		/// </summary>
		private static float GetEffectiveSpacing(Volume volume)
		{
			// Because we supply the real spacing to the VTK reslicer, the slices are interpolated
			//	as if the volume were isotropic. This results in an effective spacing that is the
			//	minimum spacing for the volume.
			return volume.MinSpacing;
		}

		#endregion
	}
}