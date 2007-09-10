using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    public class RegistrationWorklistTable : Table<RegistrationWorklistItem>
    {
        public RegistrationWorklistTable()
        {
            TableColumn<RegistrationWorklistItem, IconSet> priorityColumn = new TableColumn<RegistrationWorklistItem, IconSet>(
                SR.ColumnPriority, delegate(RegistrationWorklistItem item) { return GetOrderPriorityIcon(item.OrderPriority.Code); }, 0.5f);

            priorityColumn.Comparison = delegate(RegistrationWorklistItem item1, RegistrationWorklistItem item2)
                                            {
                                                return GetOrderPriorityIndex(item1.OrderPriority.Code) - GetOrderPriorityIndex(item2.OrderPriority.Code);
                                            };

            priorityColumn.ResourceResolver = new ResourceResolver(this.GetType().Assembly);

            this.Columns.Add(priorityColumn);
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnPatientClass,
                delegate(RegistrationWorklistItem item) { return item.PatientClass; }, 0.5f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnSite,
                delegate(RegistrationWorklistItem item) { return item.Mrn.AssigningAuthority; }, 0.5f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnMRN,
                delegate(RegistrationWorklistItem item) { return item.Mrn.Id; }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnName,
                delegate(RegistrationWorklistItem item) { return PersonNameFormat.Format(item.Name); }, 1.5f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnHealthcardNumber,
                delegate(RegistrationWorklistItem item) { return item.Healthcard.Id; }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnDateOfBirth,
                delegate(RegistrationWorklistItem item) { return Format.Date(item.DateOfBirth); }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnSex,
                delegate(RegistrationWorklistItem item) { return item.Sex.Value; }, 0.5f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnScheduledFor,
                delegate(RegistrationWorklistItem item) { return Format.Time(item.EarliestScheduledTime); }, 1.0f));

            // Sort the table by Scheduled Time initially
            int sortColumnIndex = this.Columns.FindIndex(delegate(TableColumnBase<RegistrationWorklistItem> column)
                { return column.Name.Equals(SR.ColumnScheduledFor); });

            this.Sort(new TableSortParams(this.Columns[sortColumnIndex], true));
        }

        private static int GetOrderPriorityIndex(string orderPriorityCode)
        {
            if (String.IsNullOrEmpty(orderPriorityCode))
                return 0;

            switch (orderPriorityCode)
            {
                case "S": // Stats
                    return 2;
                case "A": // Urgent
                    return 1;
                default: // Routine
                    return 0;
            }
        }

        private static IconSet GetOrderPriorityIcon(string orderPriorityCode)
        {
            switch (orderPriorityCode)
            {
                case "S": // Stats
                    return new IconSet("DoubleExclamation.png");
                case "A": // Urgent
                    return new IconSet("SingleExclamation.png");
                default: 
                    return null;
            }
        }
   }
}
