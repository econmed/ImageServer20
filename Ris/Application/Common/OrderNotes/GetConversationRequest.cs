using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
    [DataContract]
    public class GetConversationRequest : DataContractBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="orderRef"></param>
        /// <param name="categoryFilters"></param>
        /// <param name="countOnly"></param>
        public GetConversationRequest(EntityRef orderRef, List<string> categoryFilters, bool countOnly)
        {
            OrderRef = orderRef;
            CategoryFilters = categoryFilters;
            CountOnly = countOnly;
        }

        /// <summary>
        /// Specified the order for which the conversation is requested.
        /// </summary>
        [DataMember]
        public EntityRef OrderRef;

        /// <summary>
        /// Optional list of categories by which the conversation will be filtered.
        /// </summary>
        [DataMember]
        public List<string> CategoryFilters;

		/// <summary>
		/// Specifies that only a count of the messages in the conversation is requested, rather than the messages themselves.
		/// </summary>
		[DataMember]
    	public bool CountOnly;

    }
}
