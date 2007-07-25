using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class ListWorklistsResponse : DataContractBase
    {
        public ListWorklistsResponse()
        {
            WorklistSummaries = new List<WorklistAdminSummary>();
        }

        [DataMember]
        public List<WorklistAdminSummary> WorklistSummaries;
    }
}