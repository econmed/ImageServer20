using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class PlaceOrderResponse : DataContractBase
    {
        public PlaceOrderResponse(EntityRef orderRef)
        {
            this.OrderRef = orderRef;
        }

        [DataMember]
        public EntityRef OrderRef;
    }
}
