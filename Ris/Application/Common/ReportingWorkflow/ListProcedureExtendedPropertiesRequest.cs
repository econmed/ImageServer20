using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class ListProcedureExtendedPropertiesRequest : DataContractBase
	{
		public ListProcedureExtendedPropertiesRequest(EntityRef procedureRef)
		{
			ProcedureRef = procedureRef;
		}

		[DataMember]
		public EntityRef ProcedureRef;
	}
}