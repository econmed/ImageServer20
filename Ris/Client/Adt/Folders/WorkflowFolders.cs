using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Client.Adt.Folders
{
    public class ScheduledFolder : RegistrationWorkflowFolder
    {
        public ScheduledFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Scheduled")
        {

        }

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            //criteria.Scheduling.StartTime.Between(DateTime.Today, DateTime.Today.AddDays(1));
            criteria.State.EqualTo(ActivityStatus.SC);

            return ConvertToWorkListItem(this.WorkflowService.GetWorklist(criteria));
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            return item.HasStatus(ActivityStatus.SC);
        }
    }

    public class CheckedInFolder : RegistrationWorkflowFolder
    {
        public CheckedInFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Checked In")
        {

        }

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            
            // TODO: We don't have a Check-in status yet... so we can't query the Activity Status
            //criteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
            //criteria.State.EqualTo(ActivityStatus.IP);

            return ConvertToWorkListItem(this.WorkflowService.GetWorklist(criteria));
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            return item.HasStatus(ActivityStatus.IP);
        }

        protected override bool CanAcceptDrop(RegistrationWorklistItem item)
        {
            return item.HasStatus(ActivityStatus.SC);
        }

        protected override bool ConfirmAcceptDrop(ICollection<RegistrationWorklistItem> items)
        {
            DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to check in these patients?", MessageBoxActions.YesNo);
            return (result == DialogBoxAction.Yes);
        }

        protected override bool ProcessDrop(RegistrationWorklistItem item)
        {
            IRegistrationWorkflowService service = ApplicationContext.GetService<IRegistrationWorkflowService>();
            //service.StartProcedureStep(item.ProcedureStep);
            return true;
        }
    }

    public class InProgressFolder : RegistrationWorkflowFolder
    {
        public InProgressFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "In Progress")
        {

        }

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            //criteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
            criteria.State.EqualTo(ActivityStatus.IP);

            return ConvertToWorkListItem(this.WorkflowService.GetWorklist(criteria));
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            return item.HasStatus(ActivityStatus.IP);
        }

        protected override bool CanAcceptDrop(RegistrationWorklistItem item)
        {
            // For Registration, we do not allow Clerks to change status to InProgress
            //return item.HasStatus(ActivityStatus.SC);
            return false;
        }
    }

    public class CompletedFolder : RegistrationWorkflowFolder
    {
        public CompletedFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Completed")
        {

        }

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            //criteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
            criteria.State.EqualTo(ActivityStatus.CM);

            return ConvertToWorkListItem(this.WorkflowService.GetWorklist(criteria));
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            return item.HasStatus(ActivityStatus.CM);
        }

        protected override bool CanAcceptDrop(RegistrationWorklistItem item)
        {
            // For Registration, we do not allow Clerks to change status to Completed
            //return item.HasStatus(ActivityStatus.IP);
            return false;
        }
    }

    public class CancelledFolder : RegistrationWorkflowFolder
    {
        public CancelledFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Cancelled")
        {

        }

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            //criteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
            criteria.State.EqualTo(ActivityStatus.DC);

            return ConvertToWorkListItem(this.WorkflowService.GetWorklist(criteria));
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            return item.HasStatus(ActivityStatus.DC);
        }

        protected override bool CanAcceptDrop(RegistrationWorklistItem item)
        {
            // For Registration, we do not allow Clerks to change status to Cancel
            //return item.HasStatus(ActivityStatus.SC);
            return false;
        }
    }
}
