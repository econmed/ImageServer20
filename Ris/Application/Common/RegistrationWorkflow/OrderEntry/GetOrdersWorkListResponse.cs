using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class GetOrdersWorkListResponse : DataContractBase
    {
        public GetOrdersWorkListResponse(List<OrderSummary> orders)
        {
            this.Orders = orders;
        }

        [DataMember]
        public List<OrderSummary> Orders;
    }
}
