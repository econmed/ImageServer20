using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client
{
    public class TelephoneNumberTable : Table<TelephoneNumber>
    {
        public TelephoneNumberTable()
        {
            IAdtService _adtService = ApplicationContext.GetService<IAdtService>();
            TelephoneEquipmentEnumTable _phoneEquipments = _adtService.GetTelephoneEquipmentEnumTable();
            TelephoneUseEnumTable _phoneUses = _adtService.GetTelephoneUseEnumTable();

            this.Columns.Add(new TableColumn<TelephoneNumber, string>(SR.ColumnType,
                delegate(TelephoneNumber t)
                {
                    if (t.Use == TelephoneUse.PRN && t.Equipment == TelephoneEquipment.CP)
                        return SR.PhoneNumberMobile;
                    else
                        return string.Format("{0}", _phoneUses[t.Use].Value);
                }, 
                1.1f)); 
            this.Columns.Add(new TableColumn<TelephoneNumber, string>(SR.ColumnNumber, 
                delegate(TelephoneNumber pn) { return Format.Custom(pn); },
                2.2f));
            this.Columns.Add(new TableColumn<TelephoneNumber, string>(SR.ColumnExpiryDate, 
                delegate(TelephoneNumber pn) { return Format.Date(pn.ValidRange.Until); }, 
                0.9f));
        }
    }
}
