using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
    [DataContract]
    public class AcknowledgeAndPostRequest : DataContractBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderRef"></param>
        /// <param name="orderNotesToAcknowledge"></param>
        /// <param name="orderNote"></param>
        public AcknowledgeAndPostRequest(EntityRef orderRef, List<EntityRef> orderNotesToAcknowledge, OrderNoteDetail orderNote)
        {
            OrderRef = orderRef;
            OrderNotesToAcknowledge = orderNotesToAcknowledge;
            OrderNote = orderNote;
        }

        /// <summary>
        /// Specifies the order to which this request pertains.
        /// </summary>
        [DataMember]
        public EntityRef OrderRef;

        /// <summary>
        /// Specifies a list of order notes to acknowledge prior replying. Optional.
        /// </summary>
        [DataMember]
        public List<EntityRef> OrderNotesToAcknowledge;

        /// <summary>
        /// Specifies a reply order note. Optional.
        /// </summary>
        [DataMember]
        public OrderNoteDetail OrderNote;

    }
}
