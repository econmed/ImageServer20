using System;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Command for inserting or updating 'ReconcileStudy' work queue entries.
    /// </summary>
    class InsertReconcileStudyCommand : ServerDatabaseCommand
    {
        #region Private Members
        private readonly ReconcileImageContext _context;
        private Model.WorkQueue _workqueue; 
        #endregion

        #region Constructors
        public InsertReconcileStudyCommand(ReconcileImageContext context)
            : base("InsertReconcileStudyCommand", true)
        {
            Platform.CheckForNullReference(context, "context");
            _context = context;
        }

        public Model.WorkQueue ReconcileStudyWorkQueueItem
        {
            get { return _workqueue; }
        }

        #endregion

        #region Overridden Protected Methods
        protected override void OnExecute(ClearCanvas.Enterprise.Core.IUpdateContext updateContext)
        {
            Platform.CheckForNullReference(updateContext, "updateContext");
            Platform.CheckForNullReference(_context, "_context");
            
            WorkQueueSettings settings = WorkQueueSettings.Instance;
			IInsertWorkQueue broker = updateContext.GetBroker<IInsertWorkQueue>();
			InsertWorkQueueParameters parameters = new InsertWorkQueueParameters();
        	parameters.WorkQueueTypeEnum = WorkQueueTypeEnum.ReconcileStudy;
            parameters.ServerPartitionKey = _context.Partition.GetKey();
            parameters.StudyStorageKey = _context.DestinationStudyLocation!=null? _context.DestinationStudyLocation.GetKey():_context.CurrentStudyLocation.GetKey();
            parameters.StudyHistoryKey = _context.History != null ? _context.History.GetKey() : null;
            parameters.SeriesInstanceUid = _context.File.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            parameters.SopInstanceUid = _context.File.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
			parameters.WorkQueuePriorityEnum = WorkQueuePriorityEnum.High;
            parameters.Extension = "dcm";

            ReconcileStudyWorkQueueData data = new ReconcileStudyWorkQueueData();
            data.StoragePath = _context.StoragePath;
            XmlDocument xmlData = new XmlDocument();
            XmlNode node = xmlData.ImportNode(XmlUtils.Serialize(data), true);
            xmlData.AppendChild(node);
            parameters.WorkQueueData = xmlData;

            DateTime now = Platform.Time;
            parameters.ScheduledTime = now;
            parameters.ExpirationTime = now.AddSeconds(settings.WorkQueueExpireDelaySeconds);

            _workqueue = broker.FindOne(parameters);
            if (_workqueue== null)
                throw new ApplicationException("Unable to insert ReconcileStudy work queue entry");

            data = XmlUtils.Deserialize < ReconcileStudyWorkQueueData>(_workqueue.Data);
            _context.StoragePath = data.StoragePath;
        }


        #endregion

    }
}
