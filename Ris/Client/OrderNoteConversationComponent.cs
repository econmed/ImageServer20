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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="OrderNoteConversationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class OrderNoteConversationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PreliminaryDiagnosisConversationComponent class.
	/// </summary>
	[AssociateView(typeof(OrderNoteConversationComponentViewExtensionPoint))]
	public class OrderNoteConversationComponent : ApplicationComponent
	{
		private class RecipientTableItem
		{
			private readonly bool _isStaffRecipient = false;
			private readonly StaffSummary _staffSummary = null;

			private readonly bool _isGroupRecipient = false;
			private readonly StaffGroupSummary _staffGroupSummary = null;

			public RecipientTableItem(StaffSummary staffSummary)
			{
				_isStaffRecipient = true;
				_staffSummary = staffSummary;
			}

			public RecipientTableItem(StaffGroupSummary staffGroupSummary)
			{
				_isGroupRecipient = true;
				_staffGroupSummary = staffGroupSummary;
			}

			public bool IsStaffRecipient
			{
				get { return _isStaffRecipient; }
			}

			public StaffSummary StaffSummary
			{
				get { return _staffSummary; }
			}

			public bool IsGroupRecipient
			{
				get { return _isGroupRecipient; }
			}

			public StaffGroupSummary StaffGroupSummary
			{
				get { return _staffGroupSummary; }
			}

			public string Name
			{
				get
				{
					if(_isStaffRecipient)
					{
						return PersonNameFormat.Format(_staffSummary.Name);
					}
					else if(_isGroupRecipient)
					{
						return _staffGroupSummary.Name;
					}
					else
					{
						return string.Empty;
					}
				}
			}

		}

		private class RecipientTable : Table<Checkable<RecipientTableItem>>
		{
			public RecipientTable()
			{
				this.Columns.Add(new TableColumn<Checkable<RecipientTableItem>, bool>(
					"Select",
					delegate(Checkable<RecipientTableItem> item) { return item.IsChecked; },
					delegate(Checkable<RecipientTableItem> item, bool value) { item.IsChecked = value; },
					0.4f));

				this.Columns.Add(new TableColumn<Checkable<RecipientTableItem>, string>(
					"Name",
					delegate(Checkable<RecipientTableItem> item) { return item.Item.Name; },
					2.0f));
			}

			public void Add(StaffSummary staff, bool selected)
			{
				if (staff == null) return;

				Checkable<RecipientTableItem> foundItem = CollectionUtils.SelectFirst(
						this.Items,
						delegate(Checkable<RecipientTableItem> existingRecipient)
						{
							return existingRecipient.Item.IsStaffRecipient
								&& string.Equals(existingRecipient.Item.StaffSummary.StaffId, staff.StaffId);
						});

				if (foundItem == null)
				{
					this.Items.Add(new Checkable<RecipientTableItem>(new RecipientTableItem(staff), selected));
				}
				else
				{
					foundItem.IsChecked |= selected;
				}
			}

			public List<StaffSummary> SelectedStaff
			{
				get
				{
					return CollectionUtils.Map<Checkable<RecipientTableItem>, StaffSummary>(
						CollectionUtils.Select(
							this.Items,
							delegate(Checkable<RecipientTableItem> item) { return item.IsChecked && item.Item.IsStaffRecipient; }),
						delegate(Checkable<RecipientTableItem> item) { return item.Item.StaffSummary; });
				}
			}

			public void Add(StaffGroupSummary group, bool selected)
			{
				if (group == null) return;

				Checkable<RecipientTableItem> foundItem = CollectionUtils.SelectFirst(
						this.Items,
						delegate(Checkable<RecipientTableItem> existingRecipient)
						{
							return existingRecipient.Item.IsGroupRecipient
								&& string.Equals(existingRecipient.Item.StaffGroupSummary.Name, group.Name);
						});

				if (foundItem == null)
				{
					this.Items.Add(new Checkable<RecipientTableItem>(new RecipientTableItem(group), selected));
				}
				else
				{
					foundItem.IsChecked |= selected;
				}
			}

			public List<StaffGroupSummary> SelectedStaffGroups
			{
				get
				{
					return CollectionUtils.Map<Checkable<RecipientTableItem>, StaffGroupSummary>(
						CollectionUtils.Select(
							this.Items,
							delegate(Checkable<RecipientTableItem> item) { return item.IsChecked && item.Item.IsGroupRecipient; }),
						delegate(Checkable<RecipientTableItem> item) { return item.Item.StaffGroupSummary; });
				}
			}
		}

		#region Private Fields

		private EntityRef _orderRef;

		private readonly List<string> _orderNoteCategories;
		private readonly OrderNoteConversationTable _notes;
		private Checkable<OrderNoteDetail> _selectedNote;

		private string _body;

		private IList<StaffGroupSummary> _onBehalfOfChoices;
		private StaffGroupSummary _onBehalfOf;

		private readonly RecipientTable _recipients;

		private ILookupHandler _staffLookupHandler;
		private StaffSummary _selectedStaff = null;

		private ILookupHandler _staffGroupLookupHandler;
		private StaffGroupSummary _selectedStaffGroup = null;

		private ICannedTextLookupHandler _cannedTextLookupHandler;

		#endregion

		#region Constructors

		public OrderNoteConversationComponent(EntityRef orderRef, string orderNoteCategory)
			: this(orderRef, new string[] { orderNoteCategory })
		{
		}

		public OrderNoteConversationComponent(EntityRef orderRef, IEnumerable<string> orderNoteCategories)
		{
			_orderRef = orderRef;

			_orderNoteCategories = orderNoteCategories != null ? new List<string>(orderNoteCategories) : new List<string>();

			_notes = new OrderNoteConversationTable();
			_notes.CheckedItemsChanged += delegate { NotifyPropertyChanged("AcknowledgeEnabled"); };

			_recipients = new RecipientTable();
		}

		#endregion

		#region ApplicationComponent overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_staffLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow);
			_staffGroupLookupHandler = new StaffGroupLookupHandler(this.Host.DesktopWindow);
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);

			Platform.GetService<IOrderNoteService>(
				delegate(IOrderNoteService service)
				{
					GetConversationEditorFormDataResponse formDataResponse = service.GetConversationEditorFormData(new GetConversationEditorFormDataRequest());
					_onBehalfOfChoices = formDataResponse.OnBehalfOfGroupChoices;

					this.OnBehalfOf = OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName;

					GetConversationRequest request = new GetConversationRequest(_orderRef, _orderNoteCategories, false);
					GetConversationResponse response = service.GetConversation(request);

					_orderRef = response.OrderRef;

					List<Checkable<OrderNoteDetail>> checkableOrderNoteDetails =
						CollectionUtils.Map<OrderNoteDetail, Checkable<OrderNoteDetail>>(
							response.OrderNotes,
							delegate(OrderNoteDetail detail)
							{
								return new Checkable<OrderNoteDetail>(detail);
							});
					checkableOrderNoteDetails.Reverse();
					_notes.Items.AddRange(checkableOrderNoteDetails);

					// Set default recipients list
					InitializeRecipients(response.OrderNotes);
				});

			base.Start();
		}

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		#endregion

		#region Presentation Model

		#region Conversation Preview

		public ITable Notes
		{
			get { return _notes; }
		}

		public ISelection SelectedNote
		{
			get { return new Selection(_selectedNote); }
			set
			{
				Checkable<OrderNoteDetail> detail = (Checkable<OrderNoteDetail>)value.Item;
				if (_selectedNote != detail)
				{
					_selectedNote = detail;
				}
			}
		}

		public string SelectedNoteBody
		{
			get { return _selectedNote != null ? _selectedNote.Item.NoteBody : ""; }
		}

		#endregion

		public string Body
		{
			get { return _body; }
			set
			{
				_body = value;
			}
		}

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}

		public IList<string> OnBehalfOfGroupChoices
		{
			get
			{
				List<string> choices = CollectionUtils.Map<StaffGroupSummary, string>(
					_onBehalfOfChoices, 
					delegate(StaffGroupSummary summary) { return summary.Name; });
				choices.Insert(0, string.Empty);
				return choices;
			}
		}

		public string OnBehalfOf
		{
			get
			{
				return _onBehalfOf != null ? _onBehalfOf.Name : string.Empty;
			}
			set
			{
				_onBehalfOf = CollectionUtils.SelectFirst(
					_onBehalfOfChoices,
					delegate(StaffGroupSummary summary) { return string.Equals(summary.Name, value); });

				OrderNoteConversationComponentSettings.Default.PreferredOnBehalfOfGroupName = _onBehalfOf != null ? _onBehalfOf.Name : string.Empty;
			}
		}

		public ITable Recipients
		{
			get { return _recipients; }
		}

		#region Staff Recipients

		public ILookupHandler StaffRecipientLookupHandler
		{
			get { return _staffLookupHandler; }
		}

		public object SelectedStaffRecipient
		{
			get { return _selectedStaff; }
			set { _selectedStaff = (StaffSummary)value; }
		}

		public bool AddStaffRecipientEnabled
		{
			get { return _selectedStaff != null; }
		}

		public void AddStaffRecipient()
		{
			if (_selectedStaff == null) return;

			_recipients.Add(_selectedStaff, true);
			NotifyPropertyChanged("Recipients");
		}

		#endregion

		#region Group Recipients

		public ILookupHandler GroupRecipientLookupHandler
		{
			get { return _staffGroupLookupHandler; }
		}

		public object SelectedGroupRecipient
		{
			get { return _selectedStaffGroup; }
			set { _selectedStaffGroup = (StaffGroupSummary)value; }
		}

		public bool AddGroupRecipientEnabled
		{
			get { return _selectedStaffGroup != null; }
		}

		public void AddGroupRecipient()
		{
			if (_selectedStaffGroup == null) return;

			_recipients.Add(_selectedStaffGroup, true);
			NotifyPropertyChanged("Recipients");
		}

		#endregion

		public string CompleteLabel
		{
			get
			{
				string label;
				if (_notes.HasUnacknowledgedNotes())
				{
					label = string.IsNullOrEmpty(_body) ? "Acknowledge" : "Acknowledge and Post";
				}
				else
				{
					label = string.IsNullOrEmpty(_body) && _notes.Items.Count != 0 ? "OK" : "Post";
				}
				return label;
			}
		}

		public bool CompleteEnabled
		{
			get
			{
				if (_notes.Items.Count == 0)
				{
					return !string.IsNullOrEmpty(_body);
				}
				else
				{
					return !_notes.HasUncheckedUnacknowledgedNotes();
				}
			}
		}

		public void OnComplete()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
			}
			//else if (ConversationIsStale())
			//{
			//    // Message box
			//    // "The conversation has been updated.  Please review the additional notes before proceeding."
			//}
			else
			{
				try
				{
					SaveChanges();

					this.Exit(ApplicationComponentExitCode.Accepted);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, "Foo", this.Host.DesktopWindow,
											delegate()
											{
												this.ExitCode = ApplicationComponentExitCode.Error;
												this.Host.Exit();
											});
				}
			}
		}

		public void OnCancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		#endregion

		#region Private Methods

		private void InitializeRecipients(IEnumerable<OrderNoteDetail> notes)
		{
			foreach (OrderNoteDetail note in notes)
			{
				if(note.OnBehalfOfGroup != null)
				{
					_recipients.Add(note.OnBehalfOfGroup, note.CanAcknowledge);
					_recipients.Add(note.Author, false);
				}
				else
				{
					_recipients.Add(note.Author, note.CanAcknowledge);
				}

				foreach (OrderNoteDetail.StaffRecipientDetail staffRecipient in note.StaffRecipients)
				{
					if (!IsStaffCurrentUser(staffRecipient.Staff))
					{
						_recipients.Add(staffRecipient.Staff, false);
					}
				}
				foreach (OrderNoteDetail.GroupRecipientDetail groupRecipient in note.GroupRecipients)
				{
					_recipients.Add(groupRecipient.Group, false);
				}
			}
		}

		private static bool IsStaffCurrentUser(StaffSummary staff)
		{
			return string.Equals(PersonNameFormat.Format(staff.Name), PersonNameFormat.Format(LoginSession.Current.FullName));
		}

		private void SaveChanges()
		{
			Platform.GetService<IOrderNoteService>(
				delegate(IOrderNoteService service)
				{
					AcknowledgeAndPostRequest request = new AcknowledgeAndPostRequest(_orderRef, GetOrderNotesToAcknowledge(), GetReply());
					service.AcknowledgeAndPost(request);
				});
		}

		private List<EntityRef> GetOrderNotesToAcknowledge()
		{
			List<Checkable<OrderNoteDetail>> selectedOrderNotes = CollectionUtils.Select<Checkable<OrderNoteDetail>>(
				_notes.Items,
				delegate(Checkable<OrderNoteDetail> checkableOrderNoteDetail) { return checkableOrderNoteDetail.IsChecked; });

			return CollectionUtils.Map<Checkable<OrderNoteDetail>, EntityRef>(
				selectedOrderNotes,
				delegate(Checkable<OrderNoteDetail> checkableOrderNoteDetail) { return checkableOrderNoteDetail.Item.OrderNoteRef; });
		}

		private OrderNoteDetail GetReply()
		{
			if (string.IsNullOrEmpty(_body)) return null;

			OrderNoteDetail reply = new OrderNoteDetail(
				OrderNoteCategory.PreliminaryDiagnosis.Key, 
				_body, 
				_onBehalfOf, 
				_recipients.SelectedStaff, 
				_recipients.SelectedStaffGroups);

			return reply;
		}

		#endregion
	}
}
