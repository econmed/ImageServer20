using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Model
{
    public class Study : ServerEntity
    {
        #region Constructors
        public Study()
            : base("Study")
        {
        }
        #endregion

        #region Private Members
        private ServerEntityKey _serverPartitionKey;
        private ServerEntityKey _patientKey;
        private String _studyInstanceUid;
        private String _patientName;
        private String _patientId;
        private String _patientsBirthDate;
        private String _patientsSex;
        private String _studyDate;
        private String _studyTime;
        private String _accessionNumber;
        private String _studyId;
        private String _studyDescription;
        private String _referringPhysiciansName;
        private int _numberOfStudyRelatedSeries;
        private int _numberOfStudyRelatedInstances;
        #endregion

        #region Public Properties
        public ServerEntityKey ServerPartitionKey
        {
            get { return _serverPartitionKey; }
            set { _serverPartitionKey = value; }
        }
        public ServerEntityKey PatientKey
        {
            get { return _patientKey; }
            set { _patientKey = value; }
        }
        public String StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }
        public String PatientName
        {
            get { return _patientName; }
            set { _patientName = value; }
        }
        public String PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }
        public String PatientsBirthDate
        {
            get { return _patientsBirthDate; }
            set { _patientsBirthDate = value; }
        }
        public String PatientsSex
        {
            get { return _patientsSex; }
            set { _patientsSex = value; }
        }
        public String StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }
        public String StudyTime
        {
            get { return _studyTime; }
            set { _studyTime = value; }
        }
        public String AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }
        public String StudyId
        {
            get { return _studyId; }
            set { _studyId = value; }
        }
        public String StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }
        public String ReferringPhysiciansName
        {
            get { return _referringPhysiciansName; }
            set { _referringPhysiciansName = value; }
        }
        public int NumberOfStudyRelatedSeries
        {
            get { return _numberOfStudyRelatedSeries; }
            set { _numberOfStudyRelatedSeries = value; }
        }
        public int NumberOfStudyRelatedInstances
        {
            get { return _numberOfStudyRelatedInstances; }
            set { _numberOfStudyRelatedInstances = value; }
        }
        #endregion
    }
}
