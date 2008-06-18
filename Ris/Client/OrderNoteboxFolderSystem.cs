using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class OrderNoteboxFolderExtensionPoint : ExtensionPoint<IFolder>
	{
	}

	[ExtensionPoint]
	public class OrderNoteboxItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class OrderNoteboxFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IOrderNoteboxItemToolContext : IWorkflowItemToolContext<OrderNoteboxItemSummary>
	{
	}

	public interface IOrderNoteboxFolderToolContext : IWorkflowFolderToolContext
	{
		void RebuildGroupFolders();
	}

	[ExtensionOf(typeof(OrderNoteboxFolderToolExtensionPoint))]
	public class OrderNoteboxRefreshTool : RefreshTool<IOrderNoteboxFolderToolContext>
	{
	}



	public class OrderNoteboxFolderSystem: WorkflowFolderSystem<
		OrderNoteboxItemSummary,
		OrderNoteboxFolderToolExtensionPoint,
		OrderNoteboxItemToolExtensionPoint>
	{
		class OrderNoteboxItemToolContext : WorkflowItemToolContext, IOrderNoteboxItemToolContext
		{
			public OrderNoteboxItemToolContext(WorkflowFolderSystem owner)
				:base(owner)
			{
			}
		}

		class OrderNoteboxFolderToolContext : WorkflowFolderToolContext, IOrderNoteboxFolderToolContext
		{
			private readonly OrderNoteboxFolderSystem _owner;

			public OrderNoteboxFolderToolContext(OrderNoteboxFolderSystem owner)
				:base(owner)
			{
				_owner = owner;
			}

			public void RebuildGroupFolders()
			{
				_owner.RebuildGroupFolders();
			}
		}

		private readonly IconSet _unacknowledgedNotesIconSet;
		private readonly string _baseTitle;

        public OrderNoteboxFolderSystem(IFolderExplorerToolContext folderExplorer)
			: base(SR.TitleOrderNoteboxFolderSystem, folderExplorer)
		{
			_unacknowledgedNotesIconSet = new IconSet("NoteUnread.png");
			_baseTitle = SR.TitleOrderNoteboxFolderSystem;

			PersonalInboxFolder inboxFolder = new PersonalInboxFolder(this);
			inboxFolder.TotalItemCountChanged += OnPrimaryFolderCountChanged;
			this.Folders.Add(inboxFolder);
			this.Folders.Add(new SentItemsFolder(this));

			RebuildGroupFolders();
		}

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new OrderNoteboxFolderToolContext(this);
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new OrderNoteboxItemToolContext(this);
		}

		public override bool SearchEnabled
		{
			// searching not currently supported
			get { return false; }
		}

        protected override SearchResultsFolder CreateSearchResultsFolder()
        {
            // searching not currently supported
            return null;
        }

		protected override string GetPreviewUrl()
		{
			return WebResourcesSettings.Default.EmergencyPhysicianOrderNoteboxFolderSystemUrl;
		}

		protected override IDictionary<string, bool> QueryOperationEnablement(ISelection selection)
		{
			return new Dictionary<string, bool>();
		}

		protected void OnPrimaryFolderCountChanged(object sender, EventArgs e)
		{
			IFolder folder = (IFolder)sender;
			this.Title = string.Format(SR.FormatOrderNoteboxFolderSystemTitle, _baseTitle, folder.TotalItemCount);
			this.TitleIcon = folder.TotalItemCount > 0 ? _unacknowledgedNotesIconSet : null;
		}

		private void RebuildGroupFolders()
		{
			List<StaffGroupSummary> groupsToShow = null;
			Platform.GetService<IOrderNoteService>(
				delegate(IOrderNoteService service)
				{
					List<EntityRef> visibleGroups = OrderNoteboxFolderSystemSettings.Default.GroupFolders.StaffGroupRefs;
					ListStaffGroupsResponse response = service.ListStaffGroups(new ListStaffGroupsRequest());

					// select those groups that are marked as visible
					groupsToShow = CollectionUtils.Select(response.StaffGroups,
						delegate(StaffGroupSummary g)
						{
							return CollectionUtils.Contains(visibleGroups,
								delegate(EntityRef groupRef) { return groupRef.Equals(g.StaffGroupRef, true); });
						});
				});

			// remove existing group folders
			CollectionUtils.Remove(this.Folders,
				delegate(IFolder f) { return f is GroupInboxFolder; });

			// add new group folders again
			foreach (StaffGroupSummary group in groupsToShow)
			{
				GroupInboxFolder groupFolder = new GroupInboxFolder(this, group);
				this.Folders.Add(groupFolder);
			}
		}
	}
}
