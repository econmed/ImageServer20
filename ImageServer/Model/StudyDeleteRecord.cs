using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    public partial class StudyDeleteRecord
    {
        public StudyDeleteRecord(ServerPartition partition, string studyInstanceUid, string accessionNumber, string patientId, string patientName)
            : base("StudyDeleteRecord")
        {
            this.ServerPartitionAE = partition.AeTitle;
            this.StudyInstanceUid = studyInstanceUid;
            this.Timestamp = Platform.Time;
            this.AccessionNumber = accessionNumber;
            this.PatientId = patientId;
            this.PatientsName = patientName;
        }
    }
}