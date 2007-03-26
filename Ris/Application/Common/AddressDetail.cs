using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class AddressDetail : DataContractBase, ICloneable 
    {
        public AddressDetail()
        {
        }

        [DataMember]
        public string Street;

        [DataMember]
        public string Unit;

        [DataMember]
        public string City;

        [DataMember]
        public string Province;

        [DataMember]
        public string PostalCode;

        [DataMember]
        public string Country;

        [DataMember]
        public EnumValueInfo Type;

        [DataMember]
        public DateTime? ValidRangeFrom;

        [DataMember]
        public DateTime? ValidRangeUntil;

        #region ICloneable Members

        public object Clone()
        {
            AddressDetail clone = new AddressDetail();
            clone.City = this.City;
            clone.Country = this.Country;
            clone.PostalCode = this.PostalCode;
            clone.Province = this.Province;
            clone.Street = this.Street;
            clone.Type = (EnumValueInfo)this.Type.Clone();
            clone.Unit = this.Unit;
            clone.ValidRangeFrom = this.ValidRangeFrom;
            clone.ValidRangeUntil = this.ValidRangeUntil;

            return clone;
        }

        #endregion
    }
}
