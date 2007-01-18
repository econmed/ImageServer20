using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// NOTE: This code is completely throwaway.  Right now, it just provides 
	/// a hard-coded text overlay, but the plan for the future is to make 
	/// the overlay completely user-configurable.
	/// </summary>
	class AnnotationLayoutManager
	{
		private string[] _AssignmentsTopLeft = 
		{
			"Dicom.Patient.PatientId",
			"Dicom.Patient.PatientsName",
			"Dicom.Patient.PatientsBirthDate",
			"Dicom.Patient.PatientsSex",
			"Dicom.PatientStudy.PatientsAge",
			"", "", "", "", "",
			"", "", "", "", ""
		};

		private string[] _AssignmentsTopRight =
		{
			"Dicom.GeneralStudy.AccessionNumber",
			"Dicom.GeneralStudy.StudyDescription",
			"Dicom.GeneralStudy.StudyId",
			"Dicom.GeneralStudy.StudyDate",
			"Dicom.GeneralStudy.StudyTime",
			"", "", "", "", "",
			"", "", "", "", ""
		};
		
		private string[] _AssignmentsBottomLeft = 
		{
			"", "", "", "", 
			"", "", "", "", 
			"Dicom.GeneralSeries.Laterality",
			"Dicom.GeneralSeries.SeriesNumber",
			"Dicom.GeneralImage.InstanceNumber",
			"Presentation.Zoom",
			"Presentation.AppliedLut",
			"Dicom.GeneralSeries.ProtocolName",
			"Dicom.GeneralSeries.SeriesDescription"
		};

		private string[] _AssignmentsBottomRight = 
		{
			"", "", "", "",
			"", "", "", "", "",
			"", "",
			"Dicom.GeneralSeries.OperatorsName",
			"Dicom.GeneralEquipment.StationName",
			"Dicom.GeneralStudy.ReferringPhysiciansName",
			"Dicom.PatientStudy.AdditionalPatientsHistory"
		};

		private IEnumerable<IAnnotationItem> _annotationItemCollection; 
		private Dictionary<string, AnnotationLayout> _annotationLayouts;

		public AnnotationLayoutManager(IEnumerable<IAnnotationItem> annotationItemCollection)
		{
			_annotationItemCollection = annotationItemCollection;
			_annotationLayouts = new Dictionary<string, AnnotationLayout>();
		}

		public AnnotationLayout GetLayout(string layoutIdentifier)
		{
			if (_annotationLayouts.ContainsKey(layoutIdentifier))
				return _annotationLayouts[layoutIdentifier];

			//right now, everything is a default layout.
			AnnotationLayout newLayout = LoadLayout(layoutIdentifier);
			_annotationLayouts.Add(layoutIdentifier, newLayout);
			return newLayout;
		}

		protected AnnotationLayout LoadLayout(string layoutIdentifier)
		{
			List<AnnotationBox> annotationBoxes = new List<AnnotationBox>();
			BasicSetupAnnotationBoxes(annotationBoxes);

			AnnotationLayout newLayout = new AnnotationLayout();

			newLayout.Identifier = layoutIdentifier;
			newLayout.AnnotationBoxes = annotationBoxes.AsReadOnly();

			return newLayout;
		}

		protected IAnnotationItem ItemFromIdentifier(string identifier)
		{
			foreach (IAnnotationItem item in _annotationItemCollection)
			{
				if (item.GetIdentifier() == identifier)
					return item;
			}

			return null;
		}

		protected void BasicSetupAnnotationBoxes(List<AnnotationBox> annotationBoxes)
		{
			int numberOfBoxesPerQuadrant = 15;
			float boxheight = 1 / 32.0F;

			float x = 0F, y = 0F, dx = 0.5F, dy = boxheight;

			//TL
			for (int i = 0; i < numberOfBoxesPerQuadrant; ++i)
			{
				dx = (i > 0) ? 0.5F : 0.4F; //make room for directional markers.

				RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
				AnnotationBox newBox = new AnnotationBox(normalizedRectangle);
				newBox.AnnotationItem = ItemFromIdentifier(_AssignmentsTopLeft[i]);

				newBox.Bold = true;
				newBox.Font = "Century Gothic";

				annotationBoxes.Add(newBox);
				y += boxheight;
			}

			y = 0.0F;
			//TR
			for (int i = 0; i < numberOfBoxesPerQuadrant; ++i)
			{
				x = (i > 0) ? 0.5F : 0.6F; //make room for directional markers.
				dx = (i > 0) ? 0.5F : 0.4F; //make room for directional markers.

				RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
				AnnotationBox newBox = new AnnotationBox(normalizedRectangle);
				newBox.AnnotationItem = ItemFromIdentifier(_AssignmentsTopRight[i]);

				newBox.Color = "OrangeRed";
				newBox.Font = "Century Gothic";
				//newBox.Italics = true;
				newBox.Justification = AnnotationBox.JustificationBehaviour.FAR;

				annotationBoxes.Add(newBox);
				y += boxheight;
			}

			x = 0F;
			y = 1.0F - boxheight;
			//BL
			for (int i = numberOfBoxesPerQuadrant - 1; i >= 0; --i)
			{
				dx = (i < (numberOfBoxesPerQuadrant - 1)) ? 0.5F : 0.4F; //make room for directional markers.

				RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
				AnnotationBox newBox = new AnnotationBox(normalizedRectangle);
				newBox.AnnotationItem = ItemFromIdentifier(_AssignmentsBottomLeft[i]);

				newBox.Color = "Cyan";
				newBox.Font = "Century Gothic";
				
				if (i < numberOfBoxesPerQuadrant - 4)
				{
					newBox.ConfigurationOptions = new AnnotationItemConfigurationOptions();
					newBox.ConfigurationOptions.ShowLabel = true;
				}

				annotationBoxes.Add(newBox); 
				y -= boxheight;
			}

			y = 1.0F - boxheight;
			//BR
			for (int i = numberOfBoxesPerQuadrant - 1; i >= 0; --i)
			{
				x = (i < (numberOfBoxesPerQuadrant - 1)) ? 0.5F : 0.6F; //make room for directional markers.
				dx = (i < (numberOfBoxesPerQuadrant - 1)) ? 0.5F : 0.4F; //make room for directional markers.

				RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
				AnnotationBox newBox = new AnnotationBox(normalizedRectangle);
				newBox.AnnotationItem = ItemFromIdentifier(_AssignmentsBottomRight[i]);

				newBox.Color = "Yellow";
				newBox.Font = "Century Gothic";
				newBox.NumberOfLines = 2; 
				newBox.Justification = AnnotationBox.JustificationBehaviour.FAR;

				annotationBoxes.Add(newBox);
				y -= boxheight;
			}

			CreateDirectionalMarkerBox(0.00F, (1F - boxheight) / 2F, 0.1F, boxheight, AnnotationBox.JustificationBehaviour.NEAR, "Presentation.DirectionalMarkers.Left", annotationBoxes);
			CreateDirectionalMarkerBox(0.90F, (1F - boxheight) / 2F, 0.1F, boxheight, AnnotationBox.JustificationBehaviour.FAR, "Presentation.DirectionalMarkers.Right", annotationBoxes);
			CreateDirectionalMarkerBox(0.45F, 0F, 0.1F, boxheight, AnnotationBox.JustificationBehaviour.CENTRE, "Presentation.DirectionalMarkers.Top", annotationBoxes);
			CreateDirectionalMarkerBox(0.45F, 1F - boxheight, 0.1F, boxheight, AnnotationBox.JustificationBehaviour.CENTRE, "Presentation.DirectionalMarkers.Bottom", annotationBoxes);
		}

		private void CreateDirectionalMarkerBox(float x, float y, float dx, float dy, AnnotationBox.JustificationBehaviour justification, string identifier, List<AnnotationBox> annotationBoxes)
		{
			RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
			AnnotationBox newBox = new AnnotationBox(normalizedRectangle);
			newBox.AnnotationItem = ItemFromIdentifier(identifier);

			newBox.Color = "White";
			newBox.Bold = true;
			newBox.Font = "Century Gothic";
			newBox.NumberOfLines = 1; 
			newBox.Justification = justification;

			annotationBoxes.Add(newBox);
		}
	}
}
