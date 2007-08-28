using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
    [DataContract]
    public class LoadExternalPractitionerEditorFormDataResponse : DataContractBase
    {
        public LoadExternalPractitionerEditorFormDataResponse(
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
