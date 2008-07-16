#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Send to Transcription", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("apply", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
    public class CompleteInterpretationForTranscriptionTool : ReportingWorkflowItemTool
    {
        public CompleteInterpretationForTranscriptionTool()
            : base("CompleteInterpretationForTranscription")
        {
        }

        public override void Initialize()
        {
            this.Context.RegisterDropHandler(typeof(Folders.Reporting.InTranscriptionFolder), this);

            base.Initialize();
        }

    	public bool Visible
    	{
			get
			{
				return ReportingSettings.Default.EnableTranscriptionWorkflow;
			}
    	}

		public event EventHandler VisibleChanged
		{
			add {}
			remove { }
		}

		protected override bool Execute(ReportingWorklistItem item)
        {
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    service.CompleteInterpretationForTranscription(new CompleteInterpretationForTranscriptionRequest(item.ProcedureStepRef));
                });

			this.Context.InvalidateFolders(typeof(Folders.Reporting.InTranscriptionFolder));

            return true;
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Send to be Verified", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
    public class CompleteInterpretationForVerificationTool : ReportingWorkflowItemTool
    {
        public CompleteInterpretationForVerificationTool()
            : base("CompleteInterpretationForVerification")
        {
        }

        public override void Initialize()
        {
            this.Context.RegisterDropHandler(typeof(Folders.Reporting.ToBeVerifiedFolder), this);

            base.Initialize();
        }
        
        protected override bool Execute(ReportingWorklistItem item)
        {
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    service.CompleteInterpretationForVerification(new CompleteInterpretationForVerificationRequest(item.ProcedureStepRef));
                });

			this.Context.InvalidateFolders(typeof(Folders.Reporting.ToBeVerifiedFolder));

            return true;
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel Report", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[LabelValueObserver("apply", "Label", "LabelChanged")]
	[ActionPermission("apply", AuthorityTokens.Workflow.Report.Cancel)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
    public class CancelReportingStepTool : ReportingWorkflowItemTool
    {
        public CancelReportingStepTool()
            : base("CancelReportingStep")
        {
        }

		public string Label
		{
			get
			{
				ReportingWorklistItem item = GetSelectedItem();
				return (item != null && item.IsAddendumStep) ? "Cancel Addendum" : "Cancel Report";
			}
		}

		public event EventHandler LabelChanged
		{
			add { this.Context.SelectionChanged += value; }
			remove { this.Context.SelectionChanged -= value; }
		}

		protected override bool Execute(ReportingWorklistItem item)
        {
			string msg = item.IsAddendumStep ? "Discard the selected addendum?" : "Discard the selected report?";


            if (this.Context.DesktopWindow.ShowMessageBox(msg, MessageBoxActions.OkCancel)
                == DialogBoxAction.Cancel)
                return false;


            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    service.CancelReportingStep(new CancelReportingStepRequest(item.ProcedureStepRef));
                });

            // no point in invalidating "to be reported" folder because its communal

            return true;
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Verify", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Verify", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.CompleteToolSmall.png", "Icons.CompleteToolMedium.png", "Icons.CompleteToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", AuthorityTokens.Workflow.Report.Create, AuthorityTokens.Workflow.Report.Verify)]
    [ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
    public class VerifyTool : ReportingWorkflowItemTool
    {
        public VerifyTool()
            : base("Verify")
        {
        }

        public override void Initialize()
        {
            this.Context.RegisterDropHandler(typeof(Folders.Reporting.VerifiedFolder), this);

            base.Initialize();
        }

        public override bool Enabled
        {
            get
            {
                return this.Context.GetOperationEnablement("CompleteInterpretationAndVerify") ||
                    this.Context.GetOperationEnablement("CompleteVerification");
            }
        }

        public override bool CanAcceptDrop(ICollection<ReportingWorklistItem> items)
        {
            return this.Context.GetOperationEnablement("CompleteInterpretationAndVerify") ||
                this.Context.GetOperationEnablement("CompleteVerification");
        }

		protected override bool Execute(ReportingWorklistItem item)
        {
            // check for a prelim diagnosis
            if(PreliminaryDiagnosis.ConversationExists(item.OrderRef))
            {
                if(PreliminaryDiagnosis.ShowConversationDialog(item.OrderRef, this.Context.DesktopWindow)
                    == ApplicationComponentExitCode.None)
                    return false;   // user cancelled out
            }

            if (item.ProcedureStepName == "Interpretation")
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.CompleteInterpretationAndVerify(new CompleteInterpretationAndVerifyRequest(item.ProcedureStepRef));
                    });
            }
            else if (item.ProcedureStepName == "Verification")
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.CompleteVerification(new CompleteVerificationRequest(item.ProcedureStepRef));
                    });
            }
            else // (item.StepType == "Transcription")
            {
                // Not defined
            }

			this.Context.InvalidateFolders(typeof(Folders.Reporting.VerifiedFolder));

            return true;
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Add Addendum", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Add Addendum", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolMedium.png", "Icons.AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", AuthorityTokens.Workflow.Report.Create)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
    public class AddendumTool : ReportingWorkflowItemTool
    {
        public AddendumTool()
            : base("CreateAddendum")
        {
        }

		protected override bool Execute(ReportingWorklistItem item)
        {
            if (ActivateIfAlreadyOpen(item))
                return true;

			ReportingWorklistItem interpretationWorklistItem = null;

            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    CreateAddendumResponse response = service.CreateAddendum(new CreateAddendumRequest(item.ProcedureRef));
                	interpretationWorklistItem = response.ReportingWorklistItem;
                });

			this.Context.InvalidateFolders(typeof(Folders.Reporting.DraftFolder));

			if (ActivateIfAlreadyOpen(interpretationWorklistItem))
				return true;

			OpenReportEditor(interpretationWorklistItem);

            return true;
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Publish", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Publish", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolMedium.png", "Icons.AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", AuthorityTokens.Development.TestPublishReport)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
    public class PublishReportTool : ReportingWorkflowItemTool
    {
        public PublishReportTool()
            : base("PublishReport")
        {
        }

		protected override bool Execute(ReportingWorklistItem item)
        {
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    service.PublishReport(new PublishReportRequest(item.ProcedureStepRef));
                });

			return true;
        }
    }
}

