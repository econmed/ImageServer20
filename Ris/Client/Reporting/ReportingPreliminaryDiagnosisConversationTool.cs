using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
	[ExtensionOf(typeof(ReportingMainWorkflowItemToolExtensionPoint))]
	public class ReportingMainWorkflowConversationTool : PreliminaryDiagnosisConversationTool<ReportingWorklistItem, IReportingWorkflowItemToolContext>
	{
		protected override EntityRef OrderRef
		{
			get { return this.SummaryItem.OrderRef; }
		}

		protected override string Title
		{
			get { return this.SummaryItem.AccessionNumber; }
		}
	}

	[ExtensionOf(typeof(ReportingOrderNoteboxItemToolExtensionPoint))]
	public class ReportingOrderNoteboxConversationTool : PreliminaryDiagnosisConversationTool<OrderNoteboxItemSummary, IOrderNoteboxItemToolContext>
	{
		protected override EntityRef OrderRef
		{
			get { return this.SummaryItem.OrderRef; }
		}

		protected override string Title
		{
			get { return this.SummaryItem.AccessionNumber; }
		}
	}
}