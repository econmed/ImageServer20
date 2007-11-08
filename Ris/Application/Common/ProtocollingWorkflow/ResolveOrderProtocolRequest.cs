using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class ResolveOrderProtocolRequest : DataContractBase
    {
        public ResolveOrderProtocolRequest(EntityRef orderRef)
        {
            OrderRef = orderRef;
        }

        [DataMember]
        public EntityRef OrderRef;
    }
}