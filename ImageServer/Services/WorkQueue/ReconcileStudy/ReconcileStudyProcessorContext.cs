using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Encapsulated the context of the image reconciliation operation.
    /// </summary>
    public class ReconcileStudyProcessorContext
    {
        #region Private Members
        private Model.WorkQueue _item;
        private ReconcileStudyWorkQueueData _data;
        private ServerPartition _partition;
        private StudyHistory _history;
        private StudyStorageLocation _existingStudyStorageLocation;
        private StudyStorageLocation _destStudyStorageLocation;
        private ServerFilesystemInfo _targetFilesystem;
        private string _destStudyInstanceUid;
        private DicomFile _reconcileImage;
        #endregion

        #region Public Properties
        /// <summary>
        /// The 'ReconcileStudy' <see cref="WorkQueue"/> item.
        /// </summary>
        public Model.WorkQueue WorkQueueItem
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        /// The server partition associated with <see cref="WorkQueueItem"/>
        /// </summary>
        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        /// <summary>
        /// The "decoded" queue data associated with <see cref="WorkQueueItem"/>
        /// </summary>
        public ReconcileStudyWorkQueueData Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// The <see cref="StudyHistory"/> associated with the <see cref="WorkQueueItem"/>
        /// </summary>
        public StudyHistory History
        {
            get { return _history; }
            set { _history = value; }
        }

        /// <summary>
        /// The <see cref="StudyStorageLocation"/> of the study that has the same Study Instance Uid.
        /// </summary>
        public StudyStorageLocation ExistingStudyStorageLocation
        {
            get { return _existingStudyStorageLocation; }
            set { _existingStudyStorageLocation = value; }
        }

        /// <summary>
        /// The <see cref="StudyStorageLocation"/> of the resultant study which the images will be reconciled to.
        /// </summary>
        public StudyStorageLocation DestStorageLocation
        {
            get { return _destStudyStorageLocation; }
            set { _destStudyStorageLocation = value; }
        }

        public ServerFilesystemInfo DestFilesystem
        {
            get { return _targetFilesystem; }
            set { _targetFilesystem = value; }
        }

        public string DestStudyInstanceUid
        {
            get { return _destStudyInstanceUid; }
            set { _destStudyInstanceUid = value; }
        }

        public DicomFile ReconcileImage
        {
            get { return _reconcileImage; }
            set { _reconcileImage = value; }
        }

        #endregion

    }

}
