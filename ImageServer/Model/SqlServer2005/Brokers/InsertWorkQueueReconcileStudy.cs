using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise.SqlServer2005;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class InsertWorkQueueReconcileStudy : ProcedureQueryBroker<WorkQueueReconcileStudyInsertParameters, Model.WorkQueue>, IInsertWorkQueueReconcileStudy
    {
        public InsertWorkQueueReconcileStudy()
            : base("InsertWorkQueueReconcileStudy")
        {
        }
    }
}
