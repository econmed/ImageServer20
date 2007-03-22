using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin
{
    [DataContract]
    public class LoadPractitionerEditorFormDataResponse : DataContractBase
    {
        public LoadPractitionerEditorFormDataResponse(
            List<EnumValueInfo> addressTypeChoices,
            List<string> addressProvinceChoices,
            List<string> addressCountryChoices,
            List<EnumValueInfo> phoneTypeChoices)
        {
            this.AddressTypeChoices = addressTypeChoices;
            this.AddressProvinceChoices = addressProvinceChoices;
            this.AddressCountryChoices = addressCountryChoices;
            this.PhoneTypeChoices = phoneTypeChoices;
        }

        [DataMember]
        public List<EnumValueInfo> AddressTypeChoices;

        [DataMember]
        public List<string> AddressProvinceChoices;

        [DataMember]
        public List<string> AddressCountryChoices;

        [DataMember]
        public List<EnumValueInfo> PhoneTypeChoices;
    }
}
