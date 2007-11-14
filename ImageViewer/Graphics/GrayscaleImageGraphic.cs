#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A grayscale <see cref="IndexedImageGraphic"/>.
	/// </summary>
	public class GrayscaleImageGraphic : IndexedImageGraphic, IModalityLutProvider, IVoiLutProvider, IColorMapProvider
	{
		private enum Luts
		{ 
			Modality = 1,
			Voi = 2,
		}

		#region Private fields

		private LutComposer _lutComposer;
		private LutFactory _lutFactory;

		private double _rescaleSlope;
		private double _rescaleIntercept;

		private IVoiLutManager _voiLutManager;

		private IColorMapManager _colorMapManager;
		private IColorMap _colorMap;

		#endregion

		#region Public constructors

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscaleImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// <para>
		/// Creates an empty grayscale image of a specific size.
		/// By default, all pixels are set to zero (i.e., black). 
		/// Useful as a canvas on which pixels can be set by the client.
		/// </para>
		/// <para>
		/// By default, the image is 16-bit unsigned with
		/// <i>bits stored = 16</i>, <i>high bit = 15</i>,
		/// <i>rescale slope = 1.0</i> and <i>rescale intercept = 0.0</i>.
		/// </para>
		/// </remarks>
		public GrayscaleImageGraphic(int rows, int columns)
			: base(rows, 
				   columns, 
				   16, /* bits allocated */
				   16, /* bits stored */
				   15, /* high bit */
				   false,
				   false) /* is signed */
		{
			Initialize(1, 0);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscaleImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated">Can be 8 or 16.</param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="isSigned"></param>
		/// <param name="rescaleSlope"></param>
		/// <param name="rescaleIntercept"></param>
		/// <param name="pixelData"></param>
		/// <remarks>
		/// Creates an grayscale image using existing pixel data.
		/// </remarks>
		public GrayscaleImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			double rescaleSlope,
			double rescaleIntercept,
			byte[] pixelData)
			: base(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
				false,
				pixelData)
		{
			Initialize(rescaleSlope, rescaleIntercept);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscaleImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="isSigned"></param>
		/// <param name="inverted"></param>
		/// <param name="rescaleSlope"></param>
		/// <param name="rescaleIntercept"></param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// Creates a grayscale image using existing pixel data but does so
		/// without ever storing a reference to the pixel data. This is necessary
		/// to ensure that pixel data can be properly garbage collected in
		/// any future memory management schemes.
		/// </remarks>
		public GrayscaleImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool inverted,
			double rescaleSlope,
			double rescaleIntercept,
			PixelDataGetter pixelDataGetter)
			: base(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
				inverted,
				pixelDataGetter)
		{
			Initialize(rescaleSlope, rescaleIntercept);
		}

		#endregion

		#region Public properties

		#region IVoiLutProvider Members

		/// <summary>
		/// Retrieves this image's <see cref="IVoiLutManager"/>.
		/// </summary>
		public IVoiLutManager VoiLutManager
		{
			get 
			{
				if (_voiLutManager == null)
					_voiLutManager = new VoiLutManager(this);

				return _voiLutManager;
			}
		}

		#endregion

		#region IColorMapProvider Members

		/// <summary>
		/// Retrieves this image's <see cref="IColorMapManager"/>.
		/// </summary>
		public IColorMapManager ColorMapManager
		{
			get
			{
				if (_colorMapManager == null)
					_colorMapManager = new ColorMapManager(this);

				return _colorMapManager;
			}
		}

		#endregion

		/// <summary>
		/// Retrieves this image's <see cref="IModalityLut"/>
		/// </summary>
		public IModalityLut ModalityLut
		{
			get
			{
				InitializeNecessaryLuts(Luts.Modality);
				return this.LutComposer.LutCollection[0] as IModalityLut; 
			}
		}

		/// <summary>
		/// Retrieves this image's Voi Lut.
		/// </summary>
		public IComposableLut VoiLut
		{
			get
			{
				InitializeNecessaryLuts(Luts.Voi);
				return this.LutComposer.LutCollection[1]; 
			}
		}

		/// <summary>
		/// The output lut composed of both the Modality and Voi Luts.
		/// </summary>
		public sealed override IComposedLut OutputLut
		{
			get
			{
				InitializeNecessaryLuts(Luts.Voi);
				return this.LutComposer;
			}
		}

		/// <summary>
		/// Retrieves this image's <see cref="IColorMap"/>.
		/// </summary>
		public sealed override IColorMap ColorMap
		{
			get
			{
				if (_colorMap == null)
					InstallColorMap(GrayscaleColorMapFactory.FactoryName);

				return _colorMap;
			}
		}

		#endregion

		#region Private properties

		private LutComposer LutComposer
		{
			get
			{
				if (_lutComposer == null)
					_lutComposer = new LutComposer();

				return _lutComposer;
			}
		}

		private LutFactory LutFactory
		{
			get
			{
				if (_lutFactory == null)
					_lutFactory = LutFactory.NewInstance;

				return _lutFactory;
			}
		}

		#endregion

		#region Disposal

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_lutFactory != null)
					_lutFactory.Dispose();

				if (_lutComposer != null)
					_lutComposer.Dispose();
			}
		}

		#endregion

		#region Private methods

		private void Initialize(double rescaleSlope, double rescaleIntercept)
		{
			_rescaleSlope = rescaleSlope <= double.Epsilon ? 1 : rescaleSlope;
			_rescaleIntercept = rescaleIntercept;
		}

		private void InitializeNecessaryLuts(Luts luts)
		{
			if (luts >= Luts.Modality && LutComposer.LutCollection.Count == 0)
			{
				IModalityLut modalityLut =
					this.LutFactory.GetModalityLutLinear(this.BitsStored, this.IsSigned, _rescaleSlope, _rescaleIntercept);
			
				this.LutComposer.LutCollection.Add(modalityLut);
			}

			if (luts >= Luts.Voi && LutComposer.LutCollection.Count == 1)
			{
				IComposableLut lut = InitialVoiLutProvider.Instance.GetLut(this.ParentPresentationImage);
				
				if (lut == null)
					lut = new MinMaxPixelCalculatedLinearLut(this.PixelData, this.ModalityLut);

				InstallVoiLut(lut);
			}
		}

		#endregion

		#region Internal Properties / Methods

		internal IEnumerable<ColorMapDescriptor> AvailableColorMaps
		{
			get
			{
				return this.LutFactory.AvailableColorMaps;
			}
		}

		internal void InstallVoiLut(IComposableLut voiLut)
		{
			Platform.CheckForNullReference(voiLut, "voiLut");

			InitializeNecessaryLuts(Luts.Modality);

			if (this.LutComposer.LutCollection.Count == 1)
			{
				this.LutComposer.LutCollection.Add(voiLut);
			}
			else
			{
				this.LutComposer.LutCollection[1] = voiLut;
			}
		}

		internal void InstallColorMap(string name)
		{
			InstallColorMap(this.LutFactory.GetColorMap(name));
		}

		internal void InstallColorMap(IColorMap colorMap)
		{
			if (_colorMap == colorMap)
				return;

			_colorMap = colorMap;
		}

		#endregion
	}
}
