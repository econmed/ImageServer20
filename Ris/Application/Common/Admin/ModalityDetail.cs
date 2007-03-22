using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class ModalityDetail : DataContractBase
    {
        public ModalityDetail(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        [DataMember]
        public string Id;

        [DataMember]
        public string Name;
    }
}
