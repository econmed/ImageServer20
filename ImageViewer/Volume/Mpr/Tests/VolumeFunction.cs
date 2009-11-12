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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	public class VolumeFunction
	{
		/// <summary>
		/// Empty void.
		/// </summary>
		public static VolumeFunction Void = new VolumeFunction("Void", (x, y, z) => 0);

		/// <summary>
		/// Nested fuzzy shells.
		/// </summary>
		public static VolumeFunction Shells = new VolumeFunction("Shells", (x, y, z) => (float) Math.Sin(0.5*Math.Sqrt(x*x + y*y + z*z)));

		/// <summary>
		/// Weird structures vaguely resembling barcodes from different angles.
		/// </summary>
		public static VolumeFunction Barcodes = new VolumeFunction("Barcodes", (x, y, z) => (float) (x*Math.Sin(y)*(2*SquarePulseTrain(z, 25) - 1)));

		/// <summary>
		/// Odd shaded stripes.
		/// </summary>
		public static VolumeFunction Stripes = new VolumeFunction("Stripes", (x, y, z) => (x*x*x - 3*x*y*y)*(2*SquarePulseTrain(z, 25) - 1));

		/// <summary>
		/// An elliptical rod stretching diagonally.
		/// </summary>
		public static VolumeFunction Rebar = new VolumeFunction("Rebar", (x, y, z) => UnitStep(5 - Distance(x, y, z + 15, z + 15)));

		/// <summary>
		/// Two elliptical rods crossing each other.
		/// </summary>
		public static VolumeFunction Duel = new VolumeFunction("Duel", (x, y, z) => UnitStep(5 - Distance(x, y, 50, 0.8f*z + 9)) + 2*UnitStep(5 - Distance(z, y, 50, 0.8f*x + 22)));

		/// <summary>
		/// Three planets at (15,15,15), (75,25,50) and (15,85,15) of radius 5.
		/// </summary>
		public static VolumeFunction Planets = new VolumeFunction("Planets", (x, y, z) => UnitStep(5 - Distance(x, y, z, 15, 15, 15)) + 2*UnitStep(5 - Distance(x, y, z, 75, 25, 50)) + 3*UnitStep(5 - Distance(x, y, z, 15, 85, 15)));

		private delegate float VolumeFunctionDelegate(float x, float y, float z);

		private readonly VolumeFunctionDelegate _function;
		private readonly string _name;

		private VolumeFunction(string name, VolumeFunctionDelegate function)
		{
			_function = function;
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}

		public virtual float Evaluate(float x, float y, float z)
		{
			return _function(x, y, z);
		}

		public VolumeFunction Normalize(int side)
		{
			return Normalize(side, side, side);
		}

		public VolumeFunction Normalize(int width, int height, int depth)
		{
			return new NormalizedVolumeFunction(width, height, depth, this);
		}

		public Volume CreateVolume(int side, bool signed)
		{
			return CreateVolume(side, side, side, signed);
		}

		public Volume CreateVolume(int width, int height, int depth, bool signed)
		{
			DicomAttributeCollection dataset = CreateMockDataset(_name, width, height, signed);
			Size3D dimensions = new Size3D(width, height, depth);
			Vector3D spacing = new Vector3D(1, 1, 1);
			Vector3D originPatient = new Vector3D(0, 0, 0);
			Matrix orientationPatient = Matrix.GetIdentity(4);
			if (signed)
			{
				short[] data = CreateSignedArray(width, height, depth);
				return new Volume(data, dimensions, spacing, originPatient, orientationPatient, dataset, short.MinValue);
			}
			else
			{
				ushort[] data = CreateUnsignedArray(width, height, depth);
				return new Volume(data, dimensions, spacing, originPatient, orientationPatient, dataset, ushort.MinValue);
			}
		}

		public ISopDataSource[] CreateSops(int width, int height, int sliceCount, bool signed)
		{
			string studyInstanceUid = DicomUid.GenerateUid().UID;
			string seriesInstanceUid = DicomUid.GenerateUid().UID;
			string frameOfReferenceUid = DicomUid.GenerateUid().UID;

			List<SimpleSopDataSource> sops = new List<SimpleSopDataSource>();
			for (int z = 0; z < sliceCount; z++)
			{
				byte[] data = new byte[width*height*2];
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						ushort value = (ushort) Math.Max(Math.Min(this.Evaluate(x, y, z), ushort.MaxValue), ushort.MinValue);
						byte[] bytes;
						if (signed)
							bytes = BitConverter.GetBytes((short) (value - 32768));
						else
							bytes = BitConverter.GetBytes(value);
						Array.Copy(bytes, 0, data, (y*width + x)*2, 2);
					}
				}

				SimpleSopDataSource sop = new SimpleSopDataSource(CreateMockDataset(_name, width, height, signed));
				sop[DicomTags.PixelData].Values = data;
				sop[DicomTags.StudyInstanceUid].SetStringValue(studyInstanceUid);
				sop[DicomTags.SeriesInstanceUid].SetStringValue(seriesInstanceUid);
				sop[DicomTags.FrameOfReferenceUid].SetStringValue(frameOfReferenceUid);
				sop[DicomTags.ImageOrientationPatient].SetStringValue(@"1\0\0\0\1\0");
				sop[DicomTags.ImagePositionPatient].SetStringValue(string.Format(@"0\0\{0}", z));
				sops.Add(sop);
			}
			return sops.ToArray();
		}

		public short[] CreateSignedArray(int width, int height, int depth)
		{
			return Array.ConvertAll(CreateUnsignedArray(width, height, depth), v => (short) (v - 32768));
		}

		public ushort[] CreateUnsignedArray(int width, int height, int depth)
		{
			ushort[] data = new ushort[width*height*depth];
			for (int z = 0; z < depth; z++)
			{
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						data[((z*height) + y)*width + x] = (ushort) Math.Max(Math.Min(this.Evaluate(x, y, z), ushort.MaxValue), ushort.MinValue);
					}
				}
			}
			return data;
		}

		private static DicomAttributeCollection CreateMockDataset(string patientName, int columns, int rows, bool signed)
		{
			DicomAttributeCollection dataset = new DicomAttributeCollection();
			dataset[DicomTags.PatientId].SetStringValue("PATIENT");
			dataset[DicomTags.PatientsName].SetStringValue(patientName);
			dataset[DicomTags.StudyId].SetStringValue("STUDY");
			dataset[DicomTags.SeriesDescription].SetStringValue("SERIES");
			dataset[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
			dataset[DicomTags.FrameOfReferenceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.PixelSpacing].SetStringValue(@"1\1");
			dataset[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
			dataset[DicomTags.BitsStored].SetInt32(0, 16);
			dataset[DicomTags.BitsAllocated].SetInt32(0, 16);
			dataset[DicomTags.HighBit].SetInt32(0, 15);
			dataset[DicomTags.PixelRepresentation].SetInt32(0, signed ? 1 : 0);
			dataset[DicomTags.Rows].SetInt32(0, rows);
			dataset[DicomTags.Columns].SetInt32(0, columns);
			dataset[DicomTags.WindowCenter].SetInt32(0, signed ? 0 : 32768);
			dataset[DicomTags.WindowWidth].SetInt32(0, 65536);
			dataset[DicomTags.WindowCenterWidthExplanation].SetString(0, "Full Window");
			return dataset;
		}

		#region Standard Functions

		private static float SquarePulseTrain(float t, float period)
		{
			return Math.Abs((int) (2*t/period) + (t >= 0 ? 1 : 0))%2;
		}

		private static float UnitStep(float t)
		{
			return t >= 0 ? 1f : 0f;
		}

		private static float Distance(float x0, float y0, float x1, float y1)
		{
			float dx = x1 - x0;
			float dy = y1 - y0;
			return (float) Math.Sqrt(dx*dx + dy*dy);
		}

		private static float Distance(float x0, float y0, float z0, float x1, float y1, float z1)
		{
			float dx = x1 - x0;
			float dy = y1 - y0;
			float dz = z1 - z0;
			return (float) Math.Sqrt(dx*dx + dy*dy + dz*dz);
		}

		#endregion

		#region NormalizedVolumeFunction Class

		private class NormalizedVolumeFunction : VolumeFunction
		{
			private readonly float _offset;
			private readonly float _scale;

			public NormalizedVolumeFunction(int width, int height, int depth, VolumeFunction function)
				: base(function._name, function._function)
			{
				float rangeMin = float.MaxValue;
				float rangeMax = float.MinValue;
				for (int z = 0; z < depth; z++)
				{
					for (int y = 0; y < height; y++)
					{
						for (int x = 0; x < width; x++)
						{
							float v = function._function(x, y, z);
							rangeMin = Math.Min(rangeMin, v);
							rangeMax = Math.Max(rangeMax, v);
						}
					}
				}
				_offset = -rangeMin;
				_scale = 65535/(rangeMax - rangeMin);
			}

			public override float Evaluate(float x, float y, float z)
			{
				return (base.Evaluate(x, y, z) + _offset)*_scale;
			}
		}

		#endregion

		#region SimpleSopDataSource Class

		private class SimpleSopDataSource : SopDataSource
		{
			private readonly DicomAttributeCollection _dataset;
			private readonly SimpleSopFrameData _frameData;

			public SimpleSopDataSource(DicomAttributeCollection dataset)
			{
				_dataset = dataset;
				_frameData = new SimpleSopFrameData(1, this);
			}

			public override bool IsImage
			{
				get { return true; }
			}

			protected override ISopFrameData GetFrameData(int frameNumber)
			{
				return _frameData;
			}

			public override DicomAttribute this[DicomTag tag]
			{
				get { return _dataset[tag]; }
			}

			public override DicomAttribute this[uint tag]
			{
				get { return _dataset[tag]; }
			}

			public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
			{
				return _dataset.TryGetAttribute(tag, out attribute);
			}

			public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
			{
				return _dataset.TryGetAttribute(tag, out attribute);
			}

			private class SimpleSopFrameData : SopFrameData
			{
				public SimpleSopFrameData(int frameNumber, SopDataSource parent) : base(frameNumber, parent) {}

				public override byte[] GetNormalizedPixelData()
				{
					return (byte[]) base.Parent[DicomTags.PixelData].Values;
				}

				public override byte[] GetNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber)
				{
					return new byte[0];
				}

				public override void Unload() {}
			}
		}

		#endregion
	}
}

#endif