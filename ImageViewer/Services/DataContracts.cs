using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Services
{
	[DataContract]
	public class AEInformation
	{
		private string _hostName;
		private string _aeTitle;
		private int _port;

		public AEInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public string HostName
		{
			get { return _hostName; }
			set { _hostName = value; }
		}

		[DataMember(IsRequired = true)]
		public string AETitle
		{
			get { return _aeTitle; }
			set { _aeTitle = value; }
		}

		[DataMember(IsRequired = true)]
		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}
	}

	[DataContract]
	public class StudyInformation
	{
		private string _studyInstanceUid;
		private string _patientId;
		private string _patientsName;
		private string _studyDescription;
		private DateTime _studyDate;

		public StudyInformation()
		{
		}

		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		[DataMember(IsRequired = true)]
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		[DataMember(IsRequired = true)]
		public string PatientsName
		{
			get { return _patientsName; }
			set { _patientsName = value; }
		}

		[DataMember(IsRequired = true)]
		public string StudyDescription
		{
			get { return _studyDescription; }
			set { _studyDescription = value; }
		}

		[DataMember(IsRequired = true)]
		public DateTime StudyDate
		{
			get { return _studyDate; }
			set { _studyDate = value; }
		}

		public void CopyTo(StudyInformation studyInformation)
		{
			studyInformation.StudyInstanceUid = this.StudyInstanceUid;
			studyInformation.PatientId = this.PatientId;
			studyInformation.PatientsName = this.PatientsName;
			studyInformation.StudyDescription = this.StudyDescription;
			studyInformation.StudyDate = this.StudyDate;
		}

		public void CopyFrom(StudyInformation studyInformation)
		{
			this.StudyInstanceUid = studyInformation.StudyInstanceUid;
			this.PatientId = studyInformation.PatientId;
			this.PatientsName = studyInformation.PatientsName;
			this.StudyDescription = studyInformation.StudyDescription;
			this.StudyDate = studyInformation.StudyDate;
		}

		public StudyInformation Clone()
		{
			StudyInformation clone = new StudyInformation();
			CopyTo(clone);
			return clone;
		}
	}
}
