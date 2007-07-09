using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A grayscale <see cref="IndexedImageGraphic"/>.
	/// </summary>
	public class GrayscaleImageGraphic : IndexedImageGraphic, IVOILUTLinearProvider
	{
		#region Private fields

		private LUTComposer _lutComposer;
		private LUTFactory _lutFactory;
		
		private int _minPixelValue;
		private int _maxPixelValue;

		#endregion

		#region Public constructors

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscaleImageGraphic"/>
		/// with the specified <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="imageSop"></param>
		/// <remarks>
		/// This constructor is provided for convenience in the case where
		/// the properties of <see cref="GrayscaleImageGraphic"/> are the
		/// same as that of an existing <see cref="ImageSop"/>.
		/// Note that a reference to <paramref name="imageSop"/> is <i>not</i> held
		/// by <see cref="GrayscaleImageGraphic"/>.
		/// </remarks>
		public GrayscaleImageGraphic(ImageSop imageSop) 
			: this(
			imageSop.Rows,
			imageSop.Columns,
			imageSop.BitsAllocated,
			imageSop.BitsStored,
			imageSop.HighBit,
			imageSop.PixelRepresentation != 0 ? true : false,
			imageSop.PhotometricInterpretation == PhotometricInterpretation.Monochrome1 ? true: false,
			imageSop.RescaleSlope,
			imageSop.RescaleIntercept,
			imageSop.PixelData)
		{

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
		/// <param name="pixelData"></param>
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
			byte[] pixelData)
			: base(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
				pixelData)
		{
			_minPixelValue = int.MinValue;
			_maxPixelValue = int.MaxValue;

			InstallGrayscaleLUTs(rescaleSlope, rescaleIntercept, inverted);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Returns the minimum pixel value in the image pixel data itself.  Note that on first calling the get method
		/// of this property, both the minimum and maximum pixel values will be calculated, after which they are cached
		/// for performance reasons.  So, if the pixel data in this image is variable, the minimum and maximum values 
		/// must also be updated in order to remain correct.
		/// </summary>
		public virtual int MinPixelValue
		{
			get
			{
				if (_minPixelValue == int.MinValue)
					((IndexedPixelData)this.PixelData).CalculateMinMaxPixelValue(out _minPixelValue, out _maxPixelValue);

				return _minPixelValue;
			}
			protected set
			{
				Platform.CheckArgumentRange(
					value, 
					((IndexedPixelData)this.PixelData).AbsoluteMinPixelValue, 
					((IndexedPixelData)this.PixelData).AbsoluteMaxPixelValue, "value");

				_minPixelValue = value;
			}
		}

		/// <summary>
		/// Returns the maximum pixel value in the image pixel data itself.  Note that on first calling the get method
		/// of this property, both the minimum and maximum pixel values will be calculated, after which they are cached
		/// for performance reasons.  So, if the pixel data in this image is variable, the minimum and maximum values 
		/// must also be updated in order to remain correct.
		/// </summary>
		public virtual int MaxPixelValue
		{
			get
			{
				if (_maxPixelValue == int.MaxValue)
					((IndexedPixelData)this.PixelData).CalculateMinMaxPixelValue(out _minPixelValue, out _maxPixelValue);

				return _maxPixelValue;
			}
			protected set
			{
				Platform.CheckArgumentRange(
					value, 
					((IndexedPixelData)this.PixelData).AbsoluteMinPixelValue, 
					((IndexedPixelData)base.PixelData).AbsoluteMaxPixelValue, "value");

				_maxPixelValue = value;
			}
		}

		/// <summary>
		/// Gets the modality LUT.
		/// </summary>
		public IComposableLUT ModalityLUT
		{
			get { return this.LUTComposer.LUTCollection[0]; }
		}

		/// <summary>
		/// Gets the VOI LUT.
		/// </summary>
		public IComposableLUT VoiLUT
		{
			get { return this.LUTComposer.LUTCollection[1]; }
			protected set
			{
				Platform.CheckForNullReference(value, "value");
				this.LUTComposer.LUTCollection[1] = value; 
			}
		}

		/// <summary>
		/// Gets the presentation LUT.
		/// </summary>
		public IComposableLUT PresentationLUT
		{
			get { return this.LUTComposer.LUTCollection[2]; }
		}

		public override int[] OutputLUT
		{
			get { return this.LUTComposer.OutputLUT; }
		}

		#region IVOILUTLinearProvider Members

		/// <summary>
		/// Gets the linear VOI LUT.
		/// </summary>
		/// <value>The linear VOI LUT or <b>null</b> if the VOI LUT
		/// is not linear.</value>
		public virtual IVOILUTLinear VoiLutLinear
		{
			get { return this.VoiLUT as IVOILUTLinear; }
		}

		#endregion

		#endregion

		#region Private properties

		/// <summary>
		/// Gets the <see cref="LUTComposer"/>.
		/// </summary>
		private LUTComposer LUTComposer
		{
			get
			{
				if (_lutComposer == null)
					_lutComposer = new LUTComposer();

				return _lutComposer;
			}
		}

		private LUTFactory LUTFactory
		{
			get
			{
				if (_lutFactory == null)
					_lutFactory = LUTFactory.NewInstance;

				return _lutFactory;
			}
		}

		#endregion

		#region Disposal

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

		#region Static methods

		public static IStatefulVoiLutLinear NewVoiLutLinear(GrayscaleImageGraphic fromGraphic, IVoiLutLinearState state)
		{
			return new StatefulVoiLutLinear(state, fromGraphic.ModalityLUT.MinOutputValue, fromGraphic.ModalityLUT.MaxOutputValue);
		}

		public static IStatefulVoiLutLinear NewVoiLutLinear(GrayscaleImageGraphic fromGraphic)
		{
			return new StatefulVoiLutLinear(fromGraphic.ModalityLUT.MinOutputValue, fromGraphic.ModalityLUT.MaxOutputValue);
		}

		#endregion

		#region Private methods

		private void InstallGrayscaleLUTs(
			double rescaleSlope, 
			double rescaleIntercept,
			bool inverted)
		{
			ModalityLUTLinear modalityLut = this.LUTFactory.GetModalityLUTLinear(
				this.BitsStored,
				this.IsSigned,
				rescaleSlope,
				rescaleIntercept);

			this.LUTComposer.LUTCollection.Add(modalityLut);

			IStatefulVoiLutLinear voiLut = NewVoiLutLinear(this);
			this.LUTComposer.LUTCollection.Add(voiLut);

			PresentationLUT presentationLut = this.LUTFactory.GetPresentationLUT(
				voiLut.MinOutputValue,
				voiLut.MaxOutputValue,
				inverted);

			this.LUTComposer.LUTCollection.Add(presentationLut);
		}

		#endregion
	}
}
