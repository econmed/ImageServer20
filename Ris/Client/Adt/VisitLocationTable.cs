using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    public class VisitLocationTable : Table<VisitLocationDetail>
    {
        public VisitLocationTable()
        {
            this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
                SR.ColumnRole,
                delegate(VisitLocationDetail vl)
                {
                    return vl.Role.Value;
                },
                0.8f));
            this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
                SR.ColumnLocation,
                delegate(VisitLocationDetail vl)
                {
                    //TODO: LocationSummary formatting
                    //return vl.Location.ToString();
                    return string.Format("{0}, {1}, {2}, {3}, {4}", vl.Location.Bed, vl.Location.Room, vl.Location.Floor, vl.Location.Building, vl.Location.FacilityName);
                },
                2.5f));
            this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
                SR.ColumnStartTime,
                delegate(VisitLocationDetail vl)
                {
                    return Format.DateTime(vl.StartTime);
                },
                0.8f));
            this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
                SR.ColumnEndTime,
                delegate(VisitLocationDetail vl)
                {
                    return Format.DateTime(vl.EndTime);
                },
                0.8f));
        }
    }
}
