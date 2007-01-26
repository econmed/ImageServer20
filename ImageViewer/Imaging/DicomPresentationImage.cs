using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	// Maybe call this PresetationImage2D instead?
	public class DicomPresentationImage : PresentationImage, IAnnotationLayoutProvider
	{
		#region Private fields

		private ImageSop _imageSop;
		private LayerGroup _imageLayerGroup;
		private DicomImageLayer _imageLayer;
		private GraphicLayer _graphicLayer;
		private IAnnotationLayoutProvider _annotationLayoutProvider;

		#endregion


		public DicomPresentationImage(ImageSop imageSop)
		{
			Platform.CheckForNullReference(imageSop, "imageSop");

			_annotationLayoutProvider = new DicomFilteredAnnotationLayoutProvider(this);
			
			// For standard DICOM image, we have an image layer and an overlay layer
			_imageSop = imageSop;
			_imageLayerGroup = new LayerGroup();
			_imageLayer = new DicomImageLayer(imageSop);
			_graphicLayer = new GraphicLayer();

			_imageLayerGroup.Layers.Add(_imageLayer);
			_imageLayerGroup.Layers.Add(_graphicLayer);
			base.LayerManager.RootLayerGroup.Layers.Add(_imageLayerGroup);

			_imageLayerGroup.Selected = true;
			_imageLayer.Selected = true;
			_graphicLayer.Selected = true;

			_imageLayerGroup.SpatialTransform.PixelSpacingX = _imageSop.PixelSpacing.Column;
			_imageLayerGroup.SpatialTransform.PixelSpacingY = _imageSop.PixelSpacing.Row;
			_imageLayerGroup.SpatialTransform.SourceRectangle = new Rectangle(0, 0, _imageLayer.Columns, _imageLayer.Rows);
			_imageLayerGroup.SpatialTransform.Calculate();

			InstallDefaultLUTs();
		}

		#region Public properties

		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		public DicomImageLayer ImageLayer
		{
			get { return _imageLayer; }
		}

		public override IRenderer ImageRenderer
		{
			get
			{
				if (_imageRenderer == null)
					_imageRenderer = new DicomPresentationImageRenderer();

				return _imageRenderer;
			}
		}

		#endregion

		#region Disposal

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}

		#endregion

		#region Public methods

		public override IPresentationImage Clone()
		{
			return new DicomPresentationImage(_imageSop);
		}

		public override string ToString()
		{
			return _imageSop.InstanceNumber.ToString();
		}

		public override void OnDraw(DrawArgs drawArgs)
		{
			ImageLayer selectedImageLayer = this.LayerManager.SelectedImageLayer;

			if (selectedImageLayer != null)
			{
				if (selectedImageLayer.IsGrayscale && selectedImageLayer.RedrawRequired)
					this.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline.Execute();
			}

			base.OnDraw(drawArgs);
		}

		#endregion

		private void InstallDefaultLUTs()
		{
			// If the image is a colour image, then the grayscale pipeline will be null.
			if (_imageLayer.GrayscaleLUTPipeline == null)
				return;

			if (_imageLayer.GrayscaleLUTPipeline.ModalityLUT == null)
			{
				ModalityLUTLinear modalityLUT =
					new ModalityLUTLinear(
					_imageSop.BitsStored,
					_imageSop.PixelRepresentation,
					_imageSop.RescaleSlope,
					_imageSop.RescaleIntercept);

				_imageLayer.GrayscaleLUTPipeline.ModalityLUT = modalityLUT;
			}

			WindowLevelOperator.InstallVOILUTLinear(this);
		}

		#region IAnnotationLayoutProvider Members

		public virtual IAnnotationLayout AnnotationLayout
		{
			get
			{
				if (_annotationLayoutProvider == null)
					return null;

				return _annotationLayoutProvider.AnnotationLayout; 
			}
		}

		#endregion	

		public IAnnotationLayoutProvider AnnotationLayoutProvider
		{
			get { return _annotationLayoutProvider; }
			protected set { _annotationLayoutProvider = value; }
		}

	}
}
