using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.Dicom;
using System.Drawing.Drawing2D;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	/// <summary>
	/// Calculates the directional marker for a particular given edge of a viewport.  The algorithm works based
	/// on the fact that the source and destination (image & viewport) edges coincide exactly when there is no 
	/// transform applied (e.g. Left Edge corresponds to Left Edge, etc).  So, when a 2D transform is applied
	/// to a Dicom image (90 degree rotations only!) the marker at a particular edge will move to a different edge.
	/// For example, if the marker at the Left edge of the viewport is "Anterior" when there is no transformation
	/// applied, a horizontal flip will cause the "Anterior" marker to move to the Right Edge of the viewport.
	/// 
	/// The algorithm used here is quite simple:
	/// - 4 vectors, pointing to the 4 image edges (in source coordinates) are transformed to destination
	///   (viewport) coordinates.
	/// - Once in destination coordinates, the (image) edge that has now effectively moved to the viewport edge
	///   represented by this object is determined and its marker becomes the new marker for this viewport edge. 
	/// </summary>
	internal sealed class DirectionalMarkerAnnotationItem : ResourceResolvingAnnotationItem
	{
		public enum ImageEdge { Left = 0, Top = 1, Right = 2, Bottom = 3 };
		private static readonly PointF[] _edgeVectors = new PointF[] { new PointF(-1, 0), new PointF(0, -1), new PointF(1, 0), new PointF(0, 1) };

		private ImageEdge _viewportEdge;
		
		public DirectionalMarkerAnnotationItem(IAnnotationItemProvider ownerProvider, ImageEdge viewportEdge)
			: base("Presentation.DirectionalMarkers." + viewportEdge.ToString(), ownerProvider, true)
		{
			_viewportEdge = viewportEdge;
		}

		/// <summary>
		/// Gets the annotation text.
		/// </summary>
		/// <param name="presentationImage">the input presentation image.</param>
		/// <returns>the annotation text.</returns>
		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			string markerText = "";

			if (presentationImage != null)
			{
				ISpatialTransformProvider associatedTransform = presentationImage as ISpatialTransformProvider;
				IImageSopProvider associatedDicom = presentationImage as IImageSopProvider;

				if (associatedDicom != null && associatedTransform != null)
				{
					// TODO: Shouldn't have to recast.  Need to add a 2DTransform interface
					SpatialTransform spatialTransform = associatedTransform.SpatialTransform as SpatialTransform;
					if (spatialTransform != null && associatedDicom.ImageSop.ImageOrientationPatient != null)
						markerText = GetAnnotationTextInternal(spatialTransform, associatedDicom.ImageSop.ImageOrientationPatient);
				}
			}

			return markerText;
		}

		/// <summary>
		/// Called by GetAnnotationText (and also by Unit Test code).  Making this function internal simply makes it easier
		/// to write unit tests for this class (don't have to implement a fake PresentationImage).
		/// </summary>
		/// <param name="imageTransform">the image transform</param>
		/// <param name="imageOrientationPatient">the image orientation patient (direction cosines)</param>
		/// <returns></returns>
		internal string GetAnnotationTextInternal(SpatialTransform imageTransform, ImageOrientationPatient imageOrientationPatient)
		{
			PointF[] imageEdgeVectors = (PointF[])_edgeVectors.Clone();

			//convert the image vectors from source to destination.
			imageTransform.ConvertVectorsToDestination(imageEdgeVectors);

			//find out which source image edge got transformed to coincide with this viewport edge.
			ImageEdge transformedEdge = GetTransformedEdge(imageEdgeVectors);

			//get the marker for the appropriate (source) image edge.
			return GetMarker(transformedEdge, imageOrientationPatient);
		}

		/// <summary>
		/// After undergoing the transformation, the input vectors are now in Destination (viewport) coordinates.
		/// This function determines which edge in the image has moved to this edge of the viewport.
		/// That edge's original marker (when there is no transform), becomes this edge's marker.
		/// </summary>
		/// <param name="transformedVectors">source vectors transformed to the destination coordinate system</param>
		/// <returns>the source image edge that has effectively moved to this edge</returns>
		private ImageEdge GetTransformedEdge(PointF[] transformedVectors)
		{
			//the original (untransformed) vector for this viewport edge.
			PointF thisViewportEdge = _edgeVectors[(int)_viewportEdge];

			//find out which edge in the source image has moved to this edge of the viewport.
			for (int index = 0; index < transformedVectors.Length; ++index)
			{
				//normalize the vector before comparing.
				PointF transformedVector = transformedVectors[index];
				double magnitude = Math.Sqrt(Math.Pow((double)transformedVector.X, 2.0) + Math.Pow((double)transformedVector.Y, 2.0));
				transformedVector.X = (float)Math.Round(transformedVector.X / magnitude);
				transformedVector.Y = (float)Math.Round(transformedVector.Y / magnitude);

				//is it the same as the original vector for this edge?
				if (transformedVector == thisViewportEdge)
				{
					//return the image edge that has now moved to this edge of the viewport.
					return (ImageEdge)index;
				}
			}

			//this should never happen.
			throw new IndexOutOfRangeException(SR.ExceptionTransformedEdgeDoesNotMatch);
		}
		
		/// <summary>
		/// Determines the (untransformed) marker for a particular image edge.
		/// </summary>
		/// <param name="imageEdge">the edge (image coordinates)</param>
		/// <param name="imageOrientationPatient">the direction cosines of the image</param>
		/// <returns>a string representation of the direction (a 'marker')</returns>
		private string GetMarker(ImageEdge imageEdge, ImageOrientationPatient imageOrientationPatient)
		{
			bool negativeDirection = (imageEdge == ImageEdge.Left || imageEdge == ImageEdge.Top);
			bool rowValues = (imageEdge == ImageEdge.Left || imageEdge == ImageEdge.Right);

			string markerText = "";

			if (rowValues)
			{
				ImageOrientationPatient.Directions primary = imageOrientationPatient.GetPrimaryRowDirection(negativeDirection);
				ImageOrientationPatient.Directions secondary = imageOrientationPatient.GetSecondaryRowDirection(negativeDirection);
				markerText += GetMarkerText(primary);
				markerText += GetMarkerText(secondary);
			}
			else
			{
				ImageOrientationPatient.Directions primary = imageOrientationPatient.GetPrimaryColumnDirection(negativeDirection);
				ImageOrientationPatient.Directions secondary = imageOrientationPatient.GetSecondaryColumnDirection(negativeDirection);
				markerText += GetMarkerText(primary);
				markerText += GetMarkerText(secondary);
			}

			return markerText;
		}

		/// <summary>
		/// Converts an <see cref="ImageOrientationPatient.Directions"/> to a marker string.
		/// </summary>
		/// <param name="direction">the direction (patient based system)</param>
		/// <returns>marker text</returns>
		private string GetMarkerText(ImageOrientationPatient.Directions direction)
		{
			switch (direction)
			{
				case ImageOrientationPatient.Directions.Left:
					return SR.ValueDirectionalMarkersLeft;
				case ImageOrientationPatient.Directions.Right:
					return SR.ValueDirectionalMarkersRight;
				case ImageOrientationPatient.Directions.Head:
					return SR.ValueDirectionalMarkersHead;
				case ImageOrientationPatient.Directions.Foot:
					return SR.ValueDirectionalMarkersFoot;
				case ImageOrientationPatient.Directions.Anterior:
					return SR.ValueDirectionalMarkersAnterior;
				case ImageOrientationPatient.Directions.Posterior:
					return SR.ValueDirectionalMarkersPosterior;
			}

			return "";
		}
	}
}
