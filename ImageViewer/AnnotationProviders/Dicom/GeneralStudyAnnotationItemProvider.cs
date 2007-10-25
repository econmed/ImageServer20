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
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class GeneralStudyAnnotationItemProvider : AnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public GeneralStudyAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralStudy", new AnnotationResourceResolver(typeof(GeneralStudyAnnotationItemProvider).Assembly))
		{
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			if (_annotationItems == null)
			{
				_annotationItems = new List<IAnnotationItem>();

				AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralStudy.AccessionNumber",
							resolver, 
							delegate(ImageSop imageSop) { return imageSop.AccessionNumber; },
							DicomDataFormatHelper.RawStringFormat
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<PersonName>
						(
							"Dicom.GeneralStudy.ReferringPhysiciansName",
							resolver, 
							delegate(ImageSop imageSop) { return imageSop.ReferringPhysiciansName; },
							DicomDataFormatHelper.PersonNameFormatter
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralStudy.StudyDate",
							resolver, 
							delegate(ImageSop imageSop) { return imageSop.StudyDate; },
							DicomDataFormatHelper.DateFormat
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralStudy.StudyTime",
							resolver, 
							delegate(ImageSop imageSop) { return imageSop.StudyTime; },
							DicomDataFormatHelper.TimeFormat
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralStudy.StudyDescription",
							resolver, 
							delegate(ImageSop imageSop) { return imageSop.StudyDescription; },
							DicomDataFormatHelper.RawStringFormat
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralStudy.StudyId",
							resolver, 
							new DicomTagAsStringRetriever(DicomTags.StudyId).GetTagValue,
							DicomDataFormatHelper.RawStringFormat
						)
					);
			}

			return _annotationItems;
		}
	}
}
