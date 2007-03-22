using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class GetOperationEnablementRequest : DataContractBase
    {
        public GetOperationEnablementRequest(EntityRef procedureStepRef)
        {
            this.ProcedureStepRef = procedureStepRef;
        }

        [DataMember(IsRequired=true)]
        public EntityRef ProcedureStepRef;
    }
}
