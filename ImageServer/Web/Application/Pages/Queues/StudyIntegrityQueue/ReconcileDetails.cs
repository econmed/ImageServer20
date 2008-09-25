using System;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    public class ReconcileDetails
    {
        public class SeriesDetails
        {
            private string _description;
            private int _numberOfInstances;
            
            public string Description
            {
                get { return _description; }
                set { _description = value; }
            }

            public int NumberOfInstances
            {
                get { return _numberOfInstances;  }
                set { _numberOfInstances = value; }
            }
        }
        
        public class ExistingPatientInfo
        {
            private string _name;
            private SeriesDetails[] _series;

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public SeriesDetails[] Series
            {
                get { return _series; }
                set { _series = value; }                
            }
        }

        public class ConflictingPatientInfo
        {
            private string _name;
            private SeriesDetails[] _series;

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public SeriesDetails[] Series
            {
                get { return _series; }
                set { _series = value; }
            }
        }

        private string _studyInstanceUID;
        private ExistingPatientInfo _existingPatient = new ExistingPatientInfo();
        private ConflictingPatientInfo _conflictingPatient = new ConflictingPatientInfo();
        private Model.StudyIntegrityQueue _item = new Model.StudyIntegrityQueue();

        public string StudyInstanceUID
        {
            get { return _studyInstanceUID; }
            set { _studyInstanceUID = value; }
        }

        public ExistingPatientInfo ExistingPatient
        {
            get { return _existingPatient; }
            set { _existingPatient = value; }
        }
        
        public ConflictingPatientInfo ConflictingPatient
        {
            get { return _conflictingPatient; }
            set { _conflictingPatient = value; }
        }

        public Model.StudyIntegrityQueue StudyIntegrityQueueItem
        {
            get { return _item; }
            set { _item = value; }
        }
    }
}
