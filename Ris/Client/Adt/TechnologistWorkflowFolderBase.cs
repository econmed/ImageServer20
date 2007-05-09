using System;
using System.Collections.Generic;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    public abstract class TechnologistWorkflowFolderBase : WorkflowFolder<ModalityWorklistItem>
    {
        private TechnologistWorkflowFolderSystem _folderSystem;
        private IconSet _closedIconSet;
        private IconSet _openIconSet;

        private string _worklistClassName;

        public TechnologistWorkflowFolderBase(TechnologistWorkflowFolderSystem folderSystem, string folderName)
            : base(folderSystem, folderName, new ModalityWorklistTable())
        {
            _folderSystem = folderSystem;

            _closedIconSet = new IconSet(IconScheme.Colour, "FolderClosedSmall.png", "FolderClosedMedium.png", "FolderClosedMedium.png");
            _openIconSet = new IconSet(IconScheme.Colour, "FolderOpenSmall.png", "FolderOpenMedium.png", "FolderOpenMedium.png");
            this.IconSet = _closedIconSet;
            this.ResourceResolver = new ResourceResolver(this.GetType().Assembly);
        }

        public string WorklistClassName
        {
            get { return _worklistClassName; }
            set { _worklistClassName = value; }
        }

        public IconSet ClosedIconSet
        {
            get { return _closedIconSet; }
            set { _closedIconSet = value; }
        }

        public IconSet OpenIconSet
        {
            get { return _openIconSet; }
            set { _openIconSet = value; }
        }

        public override void OpenFolder()
        {
            if (_openIconSet != null)
                this.IconSet = _openIconSet;

            base.OpenFolder();
        }

        public override void CloseFolder()
        {
            if (_closedIconSet != null)
                this.IconSet = _closedIconSet;

            base.CloseFolder();
        }

        protected override bool CanQuery()
        {
            return true;
        }

        protected override int QueryCount()
        {
            int count = -1;

            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    GetWorklistCountResponse response = service.GetWorklistCount(new GetWorklistCountRequest(this.WorklistClassName));
                    count = response.ItemCount;
                });

            return count;
        }

        protected override IList<ModalityWorklistItem> QueryItems()
        {
            List<ModalityWorklistItem> worklistItems = null;

            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    GetWorklistResponse response = service.GetWorklist(new GetWorklistRequest(this.WorklistClassName));
                    worklistItems = response.WorklistItems;
                });

            return worklistItems ?? new List<ModalityWorklistItem>();
        }

        //protected override bool CanAcceptDrop(ModalityWorklistItem item)
        //{
        //    return false;
        //}

        //protected override bool ConfirmAcceptDrop(ICollection<ModalityWorklistItem> items)
        //{
        //    return true;
        //}

        //protected override bool ProcessDrop(ModalityWorklistItem item)
        //{
        //    return false;
        //}

        protected override bool IsMember(ModalityWorklistItem item)
        {
            return true;
        }
    }
}
