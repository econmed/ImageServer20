﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Protocolling;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Services.ReportingWorkflow;
using ClearCanvas.Workflow;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.ProtocollingWorkflow
{
	[ServiceImplementsContract(typeof(IProtocollingWorkflowService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class ProtocollingWorkflowService : WorkflowServiceBase, IProtocollingWorkflowService
	{
		/// <summary>
		/// Provides a context for determining if protocol operations are enabled.
		/// </summary>
		public class ProtocolOperationEnablementContext
		{
			private readonly EntityRef _procedureStepRef;
			private readonly EntityRef _orderRef;

			/// <summary>
			/// Constructor.  One of the entity refs should be non-null.
			/// </summary>
			/// <param name="orderRef"></param>
			/// <param name="procedureStepRef"></param>
			public ProtocolOperationEnablementContext(EntityRef orderRef, EntityRef procedureStepRef)
			{
				this._orderRef = orderRef;
				this._procedureStepRef = procedureStepRef;
			}

			public EntityRef OrderRef
			{
				get { return _orderRef; }
			}

			public EntityRef ProcedureStepRef
			{
				get { return _procedureStepRef; }
			}
		}

		#region IProtocollingWorkflowService Members

		[ReadOperation]
		public GetProtocolFormDataResponse GetProtocolFormData(GetProtocolFormDataRequest request)
		{
			return new GetProtocolFormDataResponse
				{
					ProtocolUrgencyChoices = EnumUtils.GetEnumValueList<ProtocolUrgencyEnum>(this.PersistenceContext)
				};
		}

		[ReadOperation]
		public GetLinkableProtocolsResponse GetLinkableProtocols(GetLinkableProtocolsRequest request)
		{
			var step = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef, EntityLoadFlags.Proxy);

			var broker = this.PersistenceContext.GetBroker<IReportingWorklistItemBroker>();
			var candidateSteps = broker.GetLinkedProtocolCandidates(step, this.CurrentUserStaff);

			// if any candidate steps were found, need to convert them to worklist items
			IList<WorklistItem> worklistItems;
			if (candidateSteps.Count > 0)
			{
				// because CLR does not support List co-variance, need to map to a list of the more general type (this seems silly!)
				var protocolSteps = CollectionUtils.Map<ProtocolAssignmentStep, ProtocolProcedureStep>(
					candidateSteps, s => s);

				worklistItems = broker.GetWorklistItems(protocolSteps);
			}
			else
			{
				worklistItems = new List<WorklistItem>();
			}

			var assembler = new ReportingWorkflowAssembler();
			return new GetLinkableProtocolsResponse(
				CollectionUtils.Map<WorklistItem, ReportingWorklistItem>(
					worklistItems, 
					item => assembler.CreateWorklistItemSummary(item, this.PersistenceContext)));
		}

		[ReadOperation]
		public ListProtocolGroupsForProcedureResponse ListProtocolGroupsForProcedure(ListProtocolGroupsForProcedureRequest request)
		{
			var assembler = new ProtocolAssembler();
			var procedure = this.PersistenceContext.Load<Procedure>(request.ProcedureRef);

			var groups = CollectionUtils.Map<ProtocolGroup, ProtocolGroupSummary>(
				this.PersistenceContext.GetBroker<IProtocolGroupBroker>().FindAll(procedure.Type),
				assembler.CreateProtocolGroupSummary);

			var initialProtocolGroup = CollectionUtils.FirstElement(groups);

			return new ListProtocolGroupsForProcedureResponse(groups, initialProtocolGroup);
		}

		[ReadOperation]
		public GetProtocolGroupDetailResponse GetProtocolGroupDetail(GetProtocolGroupDetailRequest request)
		{
			var protocolGroup = this.PersistenceContext.Load<ProtocolGroup>(request.ProtocolGroup.ProtocolGroupRef);

			var assembler = new ProtocolAssembler();

			return new GetProtocolGroupDetailResponse(assembler.CreateProtocolGroupDetail(protocolGroup, false, this.PersistenceContext));
		}

		[ReadOperation]
		public GetProcedureProtocolResponse GetProcedureProtocol(GetProcedureProtocolRequest request)
		{
			var procedure = this.PersistenceContext.Load<Procedure>(request.ProcedureRef);
			var assembler = new ProtocolAssembler();

			return procedure.ActiveProtocol != null
				? new GetProcedureProtocolResponse(assembler.CreateProtocolDetail(procedure.ActiveProtocol, this.PersistenceContext))
				: new GetProcedureProtocolResponse(null);
		}

		[ReadOperation]
		public GetProcedurePlanForProtocollingWorklistItemResponse GetProcedurePlanForProtocollingWorklistItem(GetProcedurePlanForProtocollingWorklistItemRequest request)
		{
			var order = this.PersistenceContext.Load<Order>(request.OrderRef);

			var assembler = new ProcedurePlanAssembler();
			var procedurePlanSummary = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);

			return new GetProcedurePlanForProtocollingWorklistItemResponse(procedurePlanSummary);
		}

		[ReadOperation]
		public GetSuspendRejectReasonChoicesResponse GetSuspendRejectReasonChoices(GetSuspendRejectReasonChoicesRequest request)
		{
			var choices = EnumUtils.GetEnumValueList<ProtocolRejectReasonEnum>(this.PersistenceContext);
			return new GetSuspendRejectReasonChoicesResponse(choices);
		}

		[UpdateOperation]
		[OperationEnablement("CanStartProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Create)]
		public StartProtocolResponse StartProtocol(StartProtocolRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			var protocolClaimed = false;
			var canPerformerAcceptProtocols = Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Accept);
			Staff assignedStaff = null;

			var linkedSteps = new List<ProtocolAssignmentStep>();
			if (request.LinkedProtocolAssignmentStepRefs != null && request.LinkedProtocolAssignmentStepRefs.Count > 0)
			{
				linkedSteps = CollectionUtils.Map<EntityRef, ProtocolAssignmentStep>(
					request.LinkedProtocolAssignmentStepRefs,
					stepRef => this.PersistenceContext.Load<ProtocolAssignmentStep>(stepRef));
			}

			if (request.ShouldClaim)
			{
				try
				{
					var op = new ProtocollingOperations.StartProtocolOperation();
					op.Execute(assignmentStep, linkedSteps, this.CurrentUserStaff, canPerformerAcceptProtocols, out protocolClaimed, out assignedStaff);
				}
				catch (Exception e)
				{
					throw new RequestValidationException(e.Message);
				}
			}

			var noteDetails = GetNoteDetails(assignmentStep.Procedure.Order, request.NoteCategory);

			var orderAssembler = new OrderAssembler();
			var orderDetailOptions = new OrderAssembler.CreateOrderDetailOptions(false, false, false, null, false, false, true);
			var orderDetail = orderAssembler.CreateOrderDetail(assignmentStep.Procedure.Order, orderDetailOptions, this.PersistenceContext);

			this.PersistenceContext.SynchState();

			return new StartProtocolResponse(
				assignmentStep.GetRef(),
				assignedStaff == null ? null : assignedStaff.GetRef(),
				protocolClaimed,
				noteDetails,
				orderDetail);
		}

		[UpdateOperation]
		[OperationEnablement("CanDiscardProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Create)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Cancel)]
		public DiscardProtocolResponse DiscardProtocol(DiscardProtocolRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);
			var staff = request.ReassignToStaff == null ? null : this.PersistenceContext.Load<Staff>(request.ReassignToStaff);

			// demand authority token if trying to cancel a protocol that is perfomed by someone else
			if ((assignmentStep.State == ActivityStatus.SC && !Equals(assignmentStep.AssignedStaff, this.CurrentUserStaff)) ||
				(assignmentStep.State == ActivityStatus.IP && !Equals(assignmentStep.PerformingStaff, this.CurrentUserStaff)))
			{
				var permission = new PrincipalPermission(null, AuthorityTokens.Workflow.Protocol.Cancel);
				permission.Demand();
			}

			var op = new ProtocollingOperations.DiscardProtocolOperation();
			op.Execute(assignmentStep, staff);

			this.PersistenceContext.SynchState();

			return new DiscardProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanAcceptProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Accept)]
		public AcceptProtocolResponse AcceptProtocol(AcceptProtocolRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			SaveProtocolHelper(assignmentStep, request.Protocol, request.OrderNotes, request.SupervisorRef, true);

			var op = new ProtocollingOperations.AcceptProtocolOperation();
			op.Execute(assignmentStep, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();

			return new AcceptProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanRejectProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Create)]
		public RejectProtocolResponse RejectProtocol(RejectProtocolRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			SaveProtocolHelper(assignmentStep, request.Protocol, request.OrderNotes, request.SupervisorRef, true);

			var reason = EnumUtils.GetEnumValue<ProtocolRejectReasonEnum>(request.RejectReason, this.PersistenceContext);

			var op = new ProtocollingOperations.RejectProtocolOperation();
			op.Execute(assignmentStep, this.CurrentUserStaff, reason);

			AddAdditionalCommentsNote(request.AdditionalCommentsNote, assignmentStep.Procedure.Order);

			this.PersistenceContext.SynchState();

			return new RejectProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanSaveProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Create)]
		public SaveProtocolResponse SaveProtocol(SaveProtocolRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			SaveProtocolHelper(assignmentStep, request.Protocol, request.OrderNotes, request.SupervisorRef, false);

			return new SaveProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanResubmitProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Resubmit)]
		public ResubmitProtocolResponse ResubmitProtocol(ResubmitProtocolRequest request)
		{
			var order = this.PersistenceContext.Load<Order>(request.OrderRef);

			var op = new ProtocollingOperations.ResubmitProtocolOperation();
			op.Execute(order, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();

			return new ResubmitProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanSubmitProtocolForApproval")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.SubmitForReview)]
		public SubmitProtocolForApprovalResponse SubmitProtocolForApproval(SubmitProtocolForApprovalRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			SaveProtocolHelper(assignmentStep, request.Protocol, request.OrderNotes, request.SupervisorRef, true);

			var op = new ProtocollingOperations.SubmitForApprovalOperation();
			op.Execute(assignmentStep);

			this.PersistenceContext.SynchState();

			return new SubmitProtocolForApprovalResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanReviseSubmittedProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.SubmitForReview)]
		public ReviseSubmittedProtocolResponse ReviseSubmittedProtocol(ReviseSubmittedProtocolRequest request)
		{
			var assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			var op = new ProtocollingOperations.ReviseSubmittedProtocolOperation();
			var step = op.Execute(assignmentStep, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();

			return new ReviseSubmittedProtocolResponse(GetWorklistItemSummary(step));
		}

		#endregion

		#region OperationEnablement methods

		public bool CanStartProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Create))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.StartProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanDiscardProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			// if there is no proc step ref, operation is not available
			if (enablementContext.ProcedureStepRef == null)
				return false;

			var step = this.PersistenceContext.Load<ProcedureStep>(enablementContext.ProcedureStepRef);

			var isAssignedToMe =
				(step.State == ActivityStatus.SC && Equals(step.AssignedStaff, this.CurrentUserStaff)) ||
				(step.State == ActivityStatus.IP && Equals(step.PerformingStaff, this.CurrentUserStaff));

			if (isAssignedToMe)
			{
				// Protocol is assigned to current user, allow cancel only if user has Create or Cancel token
				if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Create) &&
					!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Cancel))
					return false;
			}
			else
			{
				// Protocol not assigned to current user, allow cancel only if user has Cancel token
				if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Cancel))
					return false;
			}

			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.DiscardProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanAcceptProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Accept))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.AcceptProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanRejectProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Create))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.RejectProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanSaveProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Create))
				return false;

			if (enablementContext.ProcedureStepRef == null)
				return false;

			var step = this.PersistenceContext.Load<ProcedureStep>(enablementContext.ProcedureStepRef);

			if (!step.Is<ProtocolAssignmentStep>())
				return false;

			if (step.AssignedStaff != null && !Equals(step.AssignedStaff, this.CurrentUserStaff))
				return false;

			if (step.PerformingStaff != null && !Equals(step.PerformingStaff, CurrentUserStaff))
				return false;

			// items submitted for review should not be editable.
			var assignmentStep = step.As<ProtocolAssignmentStep>();
			if (assignmentStep.Protocol.Status == ProtocolStatus.AA)
			{
				if (Equals(assignmentStep.Protocol.Author, this.CurrentUserStaff))
					return false;
			}

			if (step.IsTerminated)
				return false;

			return true;
		}

		public bool CanResubmitProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Resubmit))
				return false;
			return CanExecuteOperation(new ProtocollingOperations.ResubmitProtocolOperation(), enablementContext.OrderRef);
		}

		public bool CanSubmitProtocolForApproval(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.SubmitForReview))
				return false;

			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.SubmitForApprovalOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanReviseSubmittedProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.SubmitForReview))
				return false;

			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.ReviseSubmittedProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		private bool CanExecuteOperation<T>(ProtocollingOperations.ProtocollingOperation op, EntityRef procedureStepRef)
			where T : ProtocolProcedureStep
		{
			// if there is no proc step ref, operation is not available
			if (procedureStepRef == null)
				return false;

			var step = this.PersistenceContext.Load<ProcedureStep>(procedureStepRef);

			if (!step.Is<T>())
				return false;

			return op.CanExecute(step.As<T>(), this.CurrentUserStaff);
		}

		private bool CanExecuteOperation(ProtocollingOperations.ProtocollingOperation op, EntityRef orderRef)
		{
			if (orderRef == null)
				return false;

			var order = this.PersistenceContext.Load<Order>(orderRef);

			return op.CanExecute(order, this.CurrentUserStaff);
		}

		#endregion

		private void SaveProtocolHelper(ProtocolAssignmentStep step, ProtocolDetail protocolDetail, List<OrderNoteDetail> notes, EntityRef supervisorRef, bool supervisorValidationRequired)
		{
			var protocol = step.Protocol;

			if (protocolDetail != null && supervisorRef != null)
				throw new RequestValidationException("UpdateProtocolRequest should not specify both a ProtocolDetail and a SupervisorRef");

			if (supervisorValidationRequired
				&& Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.OmitSupervisor) == false
				&& protocol.Supervisor == null
				&& (protocolDetail == null || protocolDetail.Supervisor == null)
				&& supervisorRef == null)
			{
				throw new SupervisorValidationException();
			}

			if (protocolDetail != null)
			{
				var assembler = new ProtocolAssembler();
				assembler.UpdateProtocol(protocol, protocolDetail, this.PersistenceContext);
			}

			if (supervisorRef != null)
			{
				var supervisor = this.PersistenceContext.Load<Staff>(supervisorRef);
				protocol.Supervisor = supervisor;
			}

			if (notes != null)
				UpdateOrderNotes(step.Procedure.Order, notes);
		}

		protected override object GetWorkItemKey(object item)
		{
			var summary = item as WorklistItemSummaryBase;
			return summary == null ? null : new ProtocolOperationEnablementContext(summary.OrderRef, summary.ProcedureStepRef);
		}

		private void AddAdditionalCommentsNote(OrderNoteDetail detail, Order order)
		{
			if (detail != null)
			{
				var noteAssembler = new OrderNoteAssembler();
				noteAssembler.CreateOrderNote(detail, order, this.CurrentUserStaff, true, this.PersistenceContext);
			}
		}

		private List<OrderNoteDetail> GetNoteDetails(Order order, string category)
		{
			var noteAssembler = new OrderNoteAssembler();

			return CollectionUtils.Map<OrderNote, OrderNoteDetail>(
				OrderNote.GetNotesForOrder(order, category),
				note => noteAssembler.CreateOrderNoteDetail(note, this.PersistenceContext));
		}

		private void UpdateOrderNotes(Order order, IList<OrderNoteDetail> notes)
		{
			var noteAssembler = new OrderNoteAssembler();

			noteAssembler.SynchronizeOrderNotes(order, notes, this.CurrentUserStaff, this.PersistenceContext);
		}

		private ReportingWorklistItem GetWorklistItemSummary(ProtocolProcedureStep reportingProcedureStep)
		{
			var procedureSteps = new List<ProtocolProcedureStep> { reportingProcedureStep };
			var items = this.PersistenceContext.GetBroker<IReportingWorklistItemBroker>().GetWorklistItems(procedureSteps);
			return new ReportingWorkflowAssembler().CreateWorklistItemSummary(CollectionUtils.FirstElement(items), this.PersistenceContext);
		}

	}
}
