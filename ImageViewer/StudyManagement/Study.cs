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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using System.Collections.ObjectModel;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM study.
	/// </summary>
	public class Study : IStudyData
	{
		private Sop _sop;
		private readonly Patient _parentPatient;
		private SeriesCollection _series;

		internal Study(Patient parentPatient)
		{
			_parentPatient = parentPatient;
		}

		/// <summary>
		/// Gets the parent <see cref="Patient"/>.
		/// </summary>
		public Patient ParentPatient
		{
			get { return _parentPatient; }
		}

		/// <summary>
		/// Gets the collection of <see cref="StudyManagement.Series"/> objects that belong
		/// to this <see cref="Study"/>.
		/// </summary>
		public SeriesCollection Series
		{
			get 
			{
				if (_series == null)
					_series = new SeriesCollection();

				return _series; 
			}
		}

		#region IStudyData Members

		/// <summary>
		/// Gets the Study Instance UID of the identified study.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return _sop.StudyInstanceUid; }
		}

		/// <summary>
		/// Gets the modalities in the identified study.
		/// </summary>
		public string[] ModalitiesInStudy
		{
			get
			{
				List<string> modalities = new List<string>();
				foreach(Series series in this.Series)
				{
					if (!modalities.Contains(series.Modality))
						modalities.Add(series.Modality);
				}
				return modalities.ToArray();
			}	
		}

		/// <summary>
		/// Gets the study description of the identified study.
		/// </summary>
		public string StudyDescription
		{
			get { return _sop.StudyDescription; }
		}

		/// <summary>
		/// Gets the study ID of the identified study.
		/// </summary>
		public string StudyId
		{
			get { return _sop.StudyId; }
		}

		/// <summary>
		/// Gets the study date of the identified study.
		/// </summary>
		public string StudyDate
		{
			get { return _sop.StudyDate; }
		}

		/// <summary>
		/// Gets the study time of the identified study.
		/// </summary>
		public string StudyTime
		{
			get { return _sop.StudyTime; }
		}

		/// <summary>
		/// Gets the accession number of the identified study.
		/// </summary>
		public string AccessionNumber
		{
			get { return _sop.AccessionNumber; }
		}

		string IStudyData.ReferringPhysiciansName
		{
			get { return _sop.ReferringPhysiciansName; }
		}

		/// <summary>
		/// Gets the number of series belonging to the identified study.
		/// </summary>
		public int NumberOfStudyRelatedSeries
		{
			get { return Series.Count; }
		}

		int? IStudyData.NumberOfStudyRelatedSeries
		{
			get { return NumberOfStudyRelatedSeries; }
		}

		/// <summary>
		/// Gets the number of composite object instances belonging to the identified study.
		/// </summary>
		public int NumberOfStudyRelatedInstances
		{
			get
			{
				int count = 0;
				foreach (Series series in Series)
					count += series.NumberOfSeriesRelatedInstances;
				return count;
			}
		}

		int? IStudyData.NumberOfStudyRelatedInstances
		{
			get { return NumberOfStudyRelatedInstances; }	
		}

		#endregion

		/// <summary>
		/// Gets the referring physician's name.
		/// </summary>
		public PersonName ReferringPhysiciansName
		{
			get { return _sop.ReferringPhysiciansName; }
		}

		/// <summary>
		/// Gets the names of physicians reading the study.
		/// </summary>
		public PersonName[] NameOfPhysiciansReadingStudy
		{
			get { return _sop.NameOfPhysiciansReadingStudy; }
		}

		/// <summary>
		/// Gets the patient's age at the time of the study.
		/// </summary>
		public string PatientsAge
		{
			get { return _sop.PatientsAge; }
		}

		/// <summary>
		/// Gets the admitting diagnoses descriptions.
		/// </summary>
		public string[] AdmittingDiagnosesDescription
		{
			get { return _sop.AdmittingDiagnosesDescription; }
		}

		/// <summary>
		/// Gets the additional patient's history.
		/// </summary>
		public string AdditionalPatientsHistory
		{
			get { return _sop.AdditionalPatientsHistory; }
		}

		/// <summary>
		/// Gets an <see cref="IStudyRootStudyIdentifier"/> for this <see cref="Study"/>.
		/// </summary>
		/// <remarks>An <see cref="IStudyRootStudyIdentifier"/> can be used in situations where you only
		/// need some data about the <see cref="Study"/>, but not the <see cref="Study"/> itself.  It can be problematic
		/// to hold references to <see cref="Study"/> objects outside the context of an <see cref="IImageViewer"/>
		/// because they are no longer valid when the viewer is closed; in these situations, it may be appropriate to
		/// use an identifier.
		/// </remarks>
		public IStudyRootStudyIdentifier GetIdentifier()
		{
			StudyItem identifier = new StudyItem(_parentPatient, this, _sop.DataSource.Server, _sop.DataSource.StudyLoaderName);
			identifier.InstanceAvailability = "ONLINE";
			return identifier;
		}

		/// <summary>
		/// Returns the study description and study instance UID associated with
		/// the <see cref="Study"/> in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = String.Format("{0} | {1}", this.StudyDescription, this.StudyInstanceUid);
			return str;
		}

		internal void SetSop(Sop sop)
		{
			_sop = sop;
			this.ParentPatient.SetSop(sop);
		}
	}
}
