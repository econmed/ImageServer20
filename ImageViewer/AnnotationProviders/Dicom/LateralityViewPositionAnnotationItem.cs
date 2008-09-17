﻿#region License

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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal class LateralityViewPositionAnnotationItem : AnnotationItem
	{
		private readonly bool _showLaterality;
		private readonly bool _showViewPosition;

		public LateralityViewPositionAnnotationItem(string identifier, bool showLaterality, bool showViewPosition)
			: base(identifier, new AnnotationResourceResolver(typeof(LateralityViewPositionAnnotationItem).Assembly))
		{
			Platform.CheckTrue(showViewPosition || showLaterality, "At least one of showLaterality and showViewPosition must be true.");

			_showLaterality = showLaterality;
			_showViewPosition = showViewPosition;
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			IImageSopProvider provider = presentationImage as IImageSopProvider;
			if (provider == null)
				return "";

			string laterality = null;
			if (_showLaterality)
			{
				laterality = provider.ImageSop[DicomTags.ImageLaterality].GetString(0, null);
				if(string.IsNullOrEmpty(laterality))
					laterality = provider.ImageSop[DicomTags.FrameLaterality].GetString(0, null);
				if (string.IsNullOrEmpty(laterality))
					laterality = provider.ImageSop[DicomTags.Laterality].GetString(0, null);
				if (string.IsNullOrEmpty(laterality))
					laterality = "?";
			}

			string viewposn = null;
			if (_showViewPosition) 
			{
				viewposn = provider.ImageSop[DicomTags.ViewPosition].GetString(0, null);
				if (string.IsNullOrEmpty(viewposn))
				{
					DicomMessageBase dmb = provider.ImageSop.NativeDicomObject;
					DicomAttributeSQ codeSeq = dmb.DataSet[DicomTags.ViewCodeSequence] as DicomAttributeSQ;

					if (codeSeq != null && codeSeq.Count > 0)
					{
						string code = codeSeq[0][DicomTags.CodeValue].GetString(0, null);
						string schm = codeSeq[0][DicomTags.CodingSchemeDesignator].GetString(0, null);
						string mean = codeSeq[0][DicomTags.CodeMeaning].GetString(0, null);

						if(code != null && schm != null)
						{
							// TODO: use a proper code sequence decoding dictionary
							viewposn = string.Format("{1}:{0}", code, schm);
							if (mean != null)
								viewposn = string.Format("{0} ({1})", viewposn, mean);
						}
					}
				}
				if (string.IsNullOrEmpty(viewposn))
					viewposn = "?";
			}

			string str = string.Empty;
			if (_showLaterality && _showViewPosition) {
				str = string.Format(SR.Dicom_GeneralImage_Composite_LateralityViewPosition_Format, laterality, viewposn);
			} else if (_showLaterality)
				str = laterality;
			else if (_showViewPosition)
				str = viewposn;
			return str;
        }
	}
}