using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class WorkQueueItemDetail : DataContractBase
	{
		[DataMember]
		public EntityRef WorkQueueItemRef;

		[DataMember]
		public DateTime CreationTime;

		[DataMember]
		public DateTime ScheduledTime;

		[DataMember]
		public DateTime? ExpirationTime;

		[DataMember]
		public string User;

		[DataMember]
		public string Type;

		[DataMember]
		public EnumValueInfo Status;

		[DataMember]
		public DateTime? ProcessedTime;

		[DataMember]
		public int FailureCount;

		[DataMember]
		public string FailureDescription;

		[DataMember]
		public Dictionary<string, string> ExtendedProperties;
	}
}