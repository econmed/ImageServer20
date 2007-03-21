using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class LoadDiagnosticServiceBreakdownRequest : DataContractBase
    {
        public LoadDiagnosticServiceBreakdownRequest(EntityRef diagnosticServiceRef)
        {
            this.DiagnosticServiceRef = diagnosticServiceRef;
        }

        [DataMember]
        public EntityRef DiagnosticServiceRef;
    }
}
