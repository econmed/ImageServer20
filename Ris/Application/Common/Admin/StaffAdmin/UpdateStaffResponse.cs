using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
    public class UpdateStaffResponse : DataContractBase
    {
        public UpdateStaffResponse(StaffSummary staff)
        {
            this.Staff = staff;
        }

        [DataMember]
        public StaffSummary Staff;
    }
}
