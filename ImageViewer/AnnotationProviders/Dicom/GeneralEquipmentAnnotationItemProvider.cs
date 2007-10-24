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
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class GeneralEquipmentAnnotationItemProvider : AnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public GeneralEquipmentAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralEquipment", new AnnotationResourceResolver(typeof(GeneralEquipmentAnnotationItemProvider).Assembly))
		{
		}

		protected override IEnumerable<IAnnotationItem> AnnotationItems
		{
			get
			{
				if (_annotationItems == null)
				{
					_annotationItems = new List<IAnnotationItem>();

					AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.DateOfLastCalibration",
								resolver,
								new DicomTagAsStringRetriever(DicomTags.DateOfLastCalibration).GetTagValue,
								DicomBasicResultFormatter.DateFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.TimeOfLastCalibration",
								resolver,
								new DicomTagAsStringRetriever(DicomTags.TimeOfLastCalibration).GetTagValue,
								DicomBasicResultFormatter.TimeFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.DeviceSerialNumber",
								resolver,
								new DicomTagAsStringRetriever(DicomTags.DeviceSerialNumber).GetTagValue,
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.InstitutionAddress",
								resolver,
								new DicomTagAsStringRetriever(DicomTags.InstitutionAddress).GetTagValue,
								DicomBasicResultFormatter.RawStringFormat
							)
						);


					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.InstitutionalDepartmentName",
								resolver,
								delegate(ImageSop imageSop) { return imageSop.InstitutionalDepartmentName; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.InstitutionName",
								resolver,
								delegate(ImageSop imageSop) { return imageSop.InstitutionName; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.Manufacturer",
								resolver,
								delegate(ImageSop imageSop) { return imageSop.Manufacturer; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.ManufacturersModelName",
								resolver,
								delegate(ImageSop imageSop) { return imageSop.ManufacturersModelName; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.StationName",
								resolver,
								delegate(ImageSop imageSop) { return imageSop.StationName; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string[]>
							(
								"Dicom.GeneralEquipment.SoftwareVersions",
								resolver,
								new DicomTagAsStringArrayRetriever(DicomTags.SoftwareVersions).GetTagValue,
								DicomBasicResultFormatter.StringListFormat
							)
						);
				}
				
				return _annotationItems;
			}
		}
	}
}
