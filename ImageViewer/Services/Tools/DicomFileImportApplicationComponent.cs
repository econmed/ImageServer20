using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.ComponentModel;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionPoint()]
	public class DicomFileImportComponentToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IDicomFileImportComponentToolContext : IToolContext
	{
		IDesktopWindow DesktopWindow { get; }

		event EventHandler SelectionUpdated;

		bool CanCancelSelected();
		bool CanClearSelected();
		bool CanClearInactive();

		void CancelSelected();
		void ClearSelected();
		void ClearInactive();
	}

	[ExtensionPoint]
	public class DicomFileImportApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(DicomFileImportApplicationComponentViewExtensionPoint))]
	public class DicomFileImportApplicationComponent : ApplicationComponent
	{
		public class DicomFileImportComponentToolContext : ToolContext, IDicomFileImportComponentToolContext
		{
			private DicomFileImportApplicationComponent _component;

			public DicomFileImportComponentToolContext(DicomFileImportApplicationComponent component)
			{
				_component = component;
			}

			#region IDicomFileImportComponentToolContext Members

			public event EventHandler SelectionUpdated
			{
				add { _component.SelectionUpdated += value; }
				remove { _component.SelectionUpdated -= value; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public bool CanCancelSelected()
			{
				return _component.SelectedCancelEnabled;
			}

			public bool CanClearSelected()
			{
				return _component.SelectedClearEnabled;
			}

			public bool CanClearInactive()
			{
				return _component.ClearInactiveEnabled;
			}

			public void CancelSelected()
			{
				_component.CancelSelected();
			}

			public void ClearSelected()
			{
				_component.ClearSelected();
			}

			public void ClearInactive()
			{
				_component.ClearInactive();
			}

			#endregion
		}

		private ToolSet _toolSet;
		private Table<ImportProgressItem> _importTable;
		private ISelection _selection;
		private ImportProgressItem _selectedProgressItem;
		private event EventHandler _selectionUpdated;

		private string _selectedStatusMessage;
		private int _selectedTotalProcessed;
		private int _selectedTotalToProcess;
		private int _selectedBadFiles;
		private int _selectedAvailableCount;
		private bool _selectedCancelEnabled;
		private bool _selectedClearEnabled;

		public DicomFileImportApplicationComponent()
		{
		}

		public event EventHandler SelectionUpdated
		{
			add { _selectionUpdated += value; }
			remove { _selectionUpdated -= value; }
		}

		public override void Start()
		{
			base.Start();

			InitializeTable();
			_toolSet = new ToolSet(new DicomFileImportComponentToolExtensionPoint(), new DicomFileImportComponentToolContext(this));

			LocalDataStoreActivityMonitor.Instance.ImportProgressUpdate += new EventHandler<ItemEventArgs<ImportProgressItem>>(OnImportProgressUpdate);
		}

		public override void Stop()
		{
			base.Stop();

			LocalDataStoreActivityMonitor.Instance.ImportProgressUpdate -= new EventHandler<ItemEventArgs<ImportProgressItem>>(OnImportProgressUpdate);
		}

		private void OnImportProgressUpdate(object sender, ItemEventArgs<ImportProgressItem> e)
		{
			int index = _importTable.Items.FindIndex(delegate(ImportProgressItem testItem)
				{
					return testItem.Identifier.Equals(e.Item.Identifier);
				});

			ImportProgressItem existingItem = null;

			if (index >= 0)
			{
				existingItem = _importTable.Items[index];
				existingItem.CopyFrom(e.Item);
				_importTable.Items.NotifyItemUpdated(index);
			}
			else
			{
				existingItem = e.Item;
				_importTable.Items.Add(e.Item);
			}

			UpateSelectedItemStats();

			if (e.Item.Removed)
			{
				if (existingItem != null)
					_importTable.Items.Remove(existingItem);
			}

			if (existingItem == _selectedProgressItem)
				EventsHelper.Fire(_selectionUpdated, this, EventArgs.Empty);
		}

		private void UpateSelectedItemStats()
		{
			if (_selectedProgressItem == null)
			{
				this.SelectedTotalProcessed = 0;
				this.SelectedAvailableCount = 0;
				this.SelectedTotalToProcess = 0;
				this.SelectedBadFiles = 0;
				this.SelectedStatusMessage = SR.MessageNothingSelected;
				this.SelectedCancelEnabled = false;
			}
			else
			{
				this.SelectedTotalToProcess = _selectedProgressItem.TotalFilesToImport;
				this.SelectedAvailableCount = _selectedProgressItem.NumberOfFilesCommittedToDataStore;
				this.SelectedBadFiles = _selectedProgressItem.NumberOfFailedImports;
				this.SelectedTotalProcessed = _selectedProgressItem.NumberOfFailedImports + _selectedProgressItem.NumberOfFilesImported;
				this.SelectedStatusMessage = _selectedProgressItem.StatusMessage;
				this.SelectedCancelEnabled = (_selectedProgressItem.AllowedCancellationOperations & CancellationFlags.Cancel) == CancellationFlags.Cancel;
				this.SelectedClearEnabled = (_selectedProgressItem.AllowedCancellationOperations & CancellationFlags.Clear) == CancellationFlags.Clear;
			}
		}

		private string FormatString(string input)
		{
			return String.IsNullOrEmpty(input) ? "" : input;
		}

		private void InitializeTable()
		{
			_importTable = new Table<ImportProgressItem>();

			TableColumnBase<ImportProgressItem> column;

			column = new TableColumn<ImportProgressItem, string>(
					SR.TitleDescription,
					delegate(ImportProgressItem item) { return FormatString(item.Description); },
					2f);

			_importTable.Columns.Add(column);

			column = new TableColumn<ImportProgressItem, string>(
					SR.TitleStartTime,
					delegate(ImportProgressItem item) 
					{
						if (item.StartTime == default(DateTime))
							return "";

						return item.StartTime.ToString(Format.TimeFormat); 
					},
					0.5f);

			_importTable.Columns.Add(column);

			column = new TableColumn<ImportProgressItem, string>(
					SR.TitleStatus,
					delegate(ImportProgressItem item) { return FormatString(item.StatusMessage); },
					2f);

			_importTable.Columns.Add(column);

		}

		private void InternalCancel(CancellationFlags flags)
		{
			if (_selectedProgressItem == null)
				return;

			List<Guid> progressIdentifiers = new List<Guid>();
			progressIdentifiers.Add(_selectedProgressItem.Identifier);

			CancelProgressItemInformation cancelInformation = new CancelProgressItemInformation();
			cancelInformation.CancellationFlags = flags;
			cancelInformation.ProgressItemIdentifiers = progressIdentifiers;

			LocalDataStoreActivityMonitor.Instance.Cancel(cancelInformation);
		}

		public ActionModelNode ToolbarModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "dicom-file-import-toolbar", _toolSet.Actions); }
		}

		public ActionModelNode ContextMenuModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "dicom-file-import-contextmenu", _toolSet.Actions); }
		}

		public string Title
		{
			get { return SR.TitleImportActivity; }
		}

		public ITable ImportTable
		{
			get { return _importTable; }
		}

		public void SetSelection(ISelection selection)
		{
			_selection = selection;
			_selectedProgressItem = (ImportProgressItem)_selection.Item;
			this.UpateSelectedItemStats();
			
			EventsHelper.Fire(_selectionUpdated, this, new ItemEventArgs<ImportProgressItem>(_selectedProgressItem));
		}

		public string SelectedStatusMessage
		{
			get
			{
				return _selectedStatusMessage;
			}
			protected set
			{
				if (value == _selectedStatusMessage)
					return;

				_selectedStatusMessage = value;
				this.NotifyPropertyChanged("SelectedStatusMessage");
			}
		}

		public int SelectedTotalProcessed
		{
			get
			{
				return _selectedTotalProcessed;
			}
			protected set
			{
				if (value == _selectedTotalProcessed)
					return;

				_selectedTotalProcessed = value;
				this.NotifyPropertyChanged("SelectedTotalProcessed");
			}
		}

		public int SelectedTotalToProcess
		{
			get
			{
				return _selectedTotalToProcess;
			}
			protected set
			{
				if (value == _selectedTotalToProcess)
					return;

				_selectedTotalToProcess = value;
				this.NotifyPropertyChanged("SelectedTotalToProcess");
			}
		}

		public int SelectedAvailableCount
		{
			get
			{
				return _selectedAvailableCount;
			}
			protected set
			{
				if (value == _selectedAvailableCount)
					return;

				_selectedAvailableCount = value;
				this.NotifyPropertyChanged("SelectedAvailableCount");
			}
		}

		public int SelectedBadFiles
		{
			get
			{
				return _selectedBadFiles;
			}
			protected set
			{
				if (value == _selectedBadFiles)
					return;

				_selectedBadFiles = value;
				this.NotifyPropertyChanged("SelectedBadFiles");
			}
		}

		public bool SelectedCancelEnabled
		{
			get { return _selectedCancelEnabled; }
			protected set
			{
				if (value == _selectedCancelEnabled)
					return;

				_selectedCancelEnabled = value;
				this.NotifyPropertyChanged("SelectedCancelEnabled");
			}
		}

		public bool SelectedClearEnabled
		{
			get { return _selectedClearEnabled; }
			protected set
			{
				if (value == _selectedClearEnabled)
					return;

				_selectedClearEnabled = value;
				this.NotifyPropertyChanged("SelectedClearEnabled");
			}
		}

		public bool ClearInactiveEnabled
		{
			get { return true; }
		}

		public void CancelSelected()
		{
			InternalCancel(CancellationFlags.Cancel);
		}

		public void ClearSelected()
		{
			InternalCancel(CancellationFlags.Clear);
		}

		public void ClearInactive()
		{
			LocalDataStoreActivityMonitor.Instance.ClearInactive();
		}
	}
}
