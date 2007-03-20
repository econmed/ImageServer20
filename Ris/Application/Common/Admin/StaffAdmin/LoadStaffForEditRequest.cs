using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
    public class LoadStaffForEditRequest : DataContractBase
    {
        public LoadStaffForEditRequest(EntityRef staffRef)
        {
            this.StaffRef = staffRef;
        }

        [DataMember]
        public EntityRef StaffRef;
    }
}
