using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint()]
	public class StudyBrowserToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint()]
	public class StudyBrowserComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public interface IStudyBrowserToolContext : IToolContext
	{
		StudyItem SelectedStudy { get; }

		ReadOnlyCollection<StudyItem> SelectedStudies { get; }

		ReadOnlyCollection<StudyItem> RelatedPriors { get; }

		AEServerGroup SelectedServerGroup { get; }

		event EventHandler SelectedStudyChanged;

		event EventHandler SelectedServerChanged;

		ClickHandlerDelegate DefaultActionHandler { get; set; }

		IDesktopWindow DesktopWindow { get; }

		void RefreshStudyList();
	}
	
	[AssociateView(typeof(StudyBrowserComponentViewExtensionPoint))]
	public class StudyBrowserComponent : ApplicationComponent
	{
		public class StudyBrowserToolContext : ToolContext, IStudyBrowserToolContext
		{
			StudyBrowserComponent _component;

			public StudyBrowserToolContext(StudyBrowserComponent component)
			{
				Platform.CheckForNullReference(component, "component");
				_component = component;
			}

			#region IStudyBrowserToolContext Members

			public StudyItem SelectedStudy
			{
				get
				{
					return _component.SelectedStudy;
				}
			}

			public ReadOnlyCollection<StudyItem> SelectedStudies 
			{
				get 
				{
					return _component.SelectedStudies;
				} 
			}

			public ReadOnlyCollection<StudyItem> RelatedPriors 
			{
				get
				{
					return _component.RelatedPriors;
				}
			}

			public AEServerGroup SelectedServerGroup
			{
				get { return _component._selectedServerGroup; }
			}

			public event EventHandler SelectedStudyChanged
			{
				add { _component.SelectedStudyChanged += value; }
				remove { _component.SelectedStudyChanged -= value; }
			}

			public event EventHandler SelectedServerChanged
			{
				add { _component.SelectedServerChanged += value; }
				remove { _component.SelectedServerChanged -= value; }
			}

			public ClickHandlerDelegate DefaultActionHandler
			{
				get { return _component._defaultActionHandler; }
				set { _component._defaultActionHandler = value; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public void RefreshStudyList()
			{
				_component.Search();
			}

			#endregion
		}

		public class SearchResult
		{
			private Table<StudyItem> _studyList;
			private string _resultsTitle = "";

			public SearchResult()
			{

			}

			public Table<StudyItem> StudyList
			{
				get 
				{
					if (_studyList == null)
						_studyList = new Table<StudyItem>();

					return _studyList; 
				}
			}

			public string ResultsTitle
			{
				get { return _resultsTitle; }
				set { _resultsTitle = value; }
			}
		}
	
		#region Fields

		private SearchPanelComponent _searchPanelComponent;

		private IStudyFinder _studyFinder;
		private Dictionary<string, SearchResult> _searchResults;
		private Table<StudyItem> _currentStudyList;
		
		private string _resultsTitle;

		private ISelection _currentSelection;
		private event EventHandler _selectedStudyChangedEvent;
		private ClickHandlerDelegate _defaultActionHandler;
		private ToolSet _toolSet;

		private AEServerGroup _selectedServerGroup;
		private event EventHandler _selectedServerChangedEvent;

		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;

		private Dictionary<string, string> _setStudiesArrived;
		#endregion

		public StudyBrowserComponent()
		{
			_searchResults = new Dictionary<string, SearchResult>();
			_setStudiesArrived = new Dictionary<string, string>();
		}

		internal SearchPanelComponent SearchPanelComponent
		{
			get { return _searchPanelComponent; }
			set { _searchPanelComponent = value; }
		}

		public Table<StudyItem> StudyList
		{
			get { return _currentStudyList; }
			set { _currentStudyList = value; }
		}

		public StudyItem SelectedStudy
		{
			get
			{
				if (_currentSelection == null)
					return null;

				return _currentSelection.Item as StudyItem;
			}
		}

		public ReadOnlyCollection<StudyItem> SelectedStudies
		{
			get
			{
				if (_currentSelection == null)
					return null;

				List<StudyItem> selectedStudies = new List<StudyItem>();

				foreach (StudyItem item in _currentSelection.Items)
					selectedStudies.Add(item);

				return selectedStudies.AsReadOnly();
			}
		}

		public ReadOnlyCollection<StudyItem> RelatedPriors
		{
			get
			{
				if (this.SelectedStudies.Count > 1)
					return null;

				// TODO

				return null;
			}
		}

		public ActionModelRoot ToolbarModel
		{
			get { return _toolbarModel; }
		}

		public ActionModelRoot ContextMenuModel
		{
			get { return _contextMenuModel; }
		}

		public string ResultsTitle
		{
			get { return _resultsTitle; }
			set
			{
				_resultsTitle = value;
				NotifyPropertyChanged("ResultsTitle");
			}
		}

		private event EventHandler SelectedStudyChanged
		{
			add { _selectedStudyChangedEvent += value; }
			remove { _selectedStudyChangedEvent -= value; }
		}

		public event EventHandler SelectedServerChanged
		{
			add { _selectedServerChangedEvent += value; }
			remove { _selectedServerChangedEvent -= value; }
		}

		#region IApplicationComponent overrides

		public override void Start()
		{
			base.Start();

			_toolSet = new ToolSet(new StudyBrowserToolExtensionPoint(), new StudyBrowserToolContext(this));
			_toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomstudybrowser-toolbar", _toolSet.Actions);
			_contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomstudybrowser-contextmenu", _toolSet.Actions);

			LocalDataStoreActivityMonitor.Instance.SopInstanceImported += new EventHandler<ItemEventArgs<ImportedSopInstanceInformation>>(OnSopInstanceImported);
			DicomExplorerConfigurationSettings.Default.PropertyChanged += new PropertyChangedEventHandler(OnConfigurationSettingsChanged);
		}

		public override void Stop()
		{
			_toolSet.Dispose();
			_toolSet = null;

			LocalDataStoreActivityMonitor.Instance.SopInstanceImported -= new EventHandler<ItemEventArgs<ImportedSopInstanceInformation>>(OnSopInstanceImported);
			DicomExplorerConfigurationSettings.Default.PropertyChanged -= new PropertyChangedEventHandler(OnConfigurationSettingsChanged);

			base.Stop();
		}

		#endregion

		public void SelectServerGroup(AEServerGroup selectedServerGroup)
		{
			_selectedServerGroup = selectedServerGroup;

			if (selectedServerGroup.IsLocalDatastore)
				_studyFinder = ImageViewerComponent.StudyFinders["DICOM_LOCAL"];
			else
				_studyFinder = ImageViewerComponent.StudyFinders["DICOM_REMOTE"];

			if (!_searchResults.ContainsKey(_selectedServerGroup.GroupID))
			{
				SearchResult searchResult = new SearchResult();
				searchResult.ResultsTitle = String.Format("{0}", _selectedServerGroup.Name);
				AddColumns(searchResult.StudyList);

				_searchResults.Add(_selectedServerGroup.GroupID, searchResult);
			}

			AddReceivedStudies();

			//Update both of these in the view.
			this.ResultsTitle = _searchResults[_selectedServerGroup.GroupID].ResultsTitle;
			this.StudyList = _searchResults[_selectedServerGroup.GroupID].StudyList;

			EventsHelper.Fire(_selectedServerChangedEvent, this, EventArgs.Empty);
		}

		public void Search()
		{
			if (_selectedServerGroup != null && _selectedServerGroup.IsLocalDatastore)
				_setStudiesArrived.Clear();

			QueryParameters queryParams = PrepareQueryParameters();

			bool isOpenSearchQuery = (queryParams["PatientsName"].Length == 0
							&& queryParams["PatientId"].Length == 0
							&& queryParams["AccessionNumber"].Length == 0
							&& queryParams["StudyDescription"].Length == 0
							&& queryParams["ModalitiesInStudy"].Length == 0
							&& queryParams["StudyDate"].Length == 0 &&
							queryParams["StudyInstanceUid"].Length == 0);


			bool isQueryingMyDataStore = (_selectedServerGroup.Servers.Count == 1 && _selectedServerGroup.Servers[0].Name == SR.TitleMyDataStore);
			if (isQueryingMyDataStore == false && isOpenSearchQuery)
			{
				if (Platform.ShowMessageBox(SR.MessageConfirmContinueOpenSearch, MessageBoxActions.YesNo) == DialogBoxAction.No)
					return;
			}

			List<KeyValuePair<string, Exception>> failedServerInfo = new List<KeyValuePair<string, Exception>>();
			StudyItemList aggregateStudyItemList = Query(queryParams, failedServerInfo);

			this.ResultsTitle = String.Format(SR.FormatStudiesFound, aggregateStudyItemList.Count, _selectedServerGroup.Name);

			//Update the results title in the component and add the new results.
			_searchResults[_selectedServerGroup.GroupID].ResultsTitle = this.ResultsTitle;
			_searchResults[_selectedServerGroup.GroupID].StudyList.Items.Clear();
			
			foreach (StudyItem item in aggregateStudyItemList)
				_searchResults[_selectedServerGroup.GroupID].StudyList.Items.Add(item);

			_searchResults[_selectedServerGroup.GroupID].StudyList.Sort();

            // Re-throw the last exception with a list of failed server name, if any
			if (failedServerInfo.Count > 0)
            {
				StringBuilder aggregateExceptionMessage = new StringBuilder();
				int count = 0;
                foreach(KeyValuePair<string, Exception> pair in failedServerInfo)
                {
					if (count++ > 0)
						aggregateExceptionMessage.Append("\n\n");

					aggregateExceptionMessage.AppendFormat(SR.FormatUnableToQueryServer, pair.Key, pair.Value.Message);
                }

				throw new Exception(aggregateExceptionMessage.ToString());
            }
		}

		public void ItemDoubleClick()
		{
			if (_defaultActionHandler != null)
			{
				_defaultActionHandler();
			}
		}

		public void SetSelection(ISelection selection)
		{
			if (_currentSelection != selection)
			{
				_currentSelection = selection;
				EventsHelper.Fire(_selectedStudyChangedEvent, this, EventArgs.Empty);
			}
		}

		private QueryParameters PrepareQueryParameters()
		{
			Platform.CheckMemberIsSet(_studyFinder, "StudyFinder");
			Platform.CheckMemberIsSet(_searchPanelComponent, "SearchPanelComponent");

			// create patient's name query key
			// LastName   FirstName   Result
			//    X           X        <Blank>
			//    V           X        LastName*
			//    V           V        LastName*FirstName*
			//    X           V        *FirstName*
			string patientsName = "";
			if (_searchPanelComponent.LastName.Length > 0 && _searchPanelComponent.FirstName.Length == 0)
				patientsName = _searchPanelComponent.LastName + "*";
			if (_searchPanelComponent.LastName.Length > 0 && _searchPanelComponent.FirstName.Length > 0)
				patientsName = _searchPanelComponent.LastName + "*" + _searchPanelComponent.FirstName + "*";
			if (_searchPanelComponent.LastName.Length == 0 && _searchPanelComponent.FirstName.Length > 0)
				patientsName = "*" + _searchPanelComponent.FirstName + "*";

			string patientId = "";
			if (_searchPanelComponent.PatientID.Length > 0)
				patientId = _searchPanelComponent.PatientID + "*";

			string accessionNumber = "";
			if (_searchPanelComponent.AccessionNumber.Length > 0)
				accessionNumber = _searchPanelComponent.AccessionNumber + "*";

			string studyDescription = "";
			if (_searchPanelComponent.StudyDescription.Length > 0)
				studyDescription = _searchPanelComponent.StudyDescription + "*";

			string dateRangeQuery = DateRangeHelper.GetDicomDateRangeQueryString(_searchPanelComponent.StudyDateFrom, _searchPanelComponent.StudyDateTo);

			//At the application level, ClearCanvas defines the 'ModalitiesInStudy' filter as a multi-valued
			//Key Attribute.  This goes against the Dicom standard for C-FIND SCU behaviour, so the
			//underlying IStudyFinder(s) must handle this special case, either by ignoring the filter
			//or by running multiple queries, one per modality specified (for example).

			string modalityFilter = VMStringConverter.ToDicomStringArray<string>(_searchPanelComponent.SearchModalities);

			QueryParameters queryParams = new QueryParameters();
			queryParams.Add("PatientsName", patientsName);
			queryParams.Add("PatientId", patientId);
			queryParams.Add("AccessionNumber", accessionNumber);
			queryParams.Add("StudyDescription", studyDescription);
			queryParams.Add("ModalitiesInStudy", modalityFilter);
			queryParams.Add("StudyDate", dateRangeQuery);
			queryParams.Add("StudyInstanceUid", "");

			return queryParams;
		}

		private StudyItemList Query(QueryParameters queryParams, List<KeyValuePair<string, Exception>> failedServerInfo)
		{
			StudyItemList aggregateStudyItemList = new StudyItemList();

			foreach (Server server in _selectedServerGroup.Servers)
			{
				try
				{
					StudyItemList serverStudyItemList = _studyFinder.Query(server.GetApplicationEntity(), queryParams);
					aggregateStudyItemList.AddRange(serverStudyItemList);
				}
				catch (Exception e)
				{
					// keep track of the failed server names and exceptions
					failedServerInfo.Add(new KeyValuePair<string, Exception>(server.Name, e));
				}
			}

			return aggregateStudyItemList;
		}

		private void AddColumns(Table<StudyItem> studyList)
		{
			TableColumn<StudyItem, string> column;

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingPatientId,
					delegate(StudyItem item) { return item.PatientId; },
					1.5f);

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingLastName,
					delegate(StudyItem item) { return item.PatientsName.LastName; },
                    1.5f);

			studyList.Columns.Add(column);

			// Default: Sort by lastname
			studyList.Sort(new TableSortParams(column, true));

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingFirstName,
					delegate(StudyItem item) { return item.PatientsName.FirstName; },
                    1.5f);

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingIdeographicName,
					delegate(StudyItem item) { return item.PatientsName.Ideographic; },
					1.5f);

			column.Visible = DicomExplorerConfigurationSettings.Default.ShowIdeographicName;

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingPhoneticName,
					delegate(StudyItem item) { return item.PatientsName.Phonetic; },
					1.5f);

			column.Visible = DicomExplorerConfigurationSettings.Default.ShowPhoneticName;

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingDateOfBirth,
                    delegate(StudyItem item) { return DicomHelper.GetDateStringFromDicomDA(item.PatientsBirthDate); },
                    null,
                    1.0f,
                    delegate(StudyItem one, StudyItem two) { return one.PatientsBirthDate.CompareTo(two.PatientsBirthDate); });

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingAccessionNumber,
					delegate(StudyItem item) { return item.AccessionNumber; });

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingStudyDate,
					delegate(StudyItem item) { return DicomHelper.GetDateStringFromDicomDA(item.StudyDate); },
                    null,
                    1.0f,
                    delegate(StudyItem one, StudyItem two) {  return one.StudyDate.CompareTo(two.StudyDate); });

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingStudyDescription,
					delegate(StudyItem item) { return item.StudyDescription; },
                    2.5f);

			studyList.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
					SR.ColumnHeadingModality,
                    delegate(StudyItem item) { return item.ModalitiesInStudy; },
                    0.5f);

			studyList.Columns.Add(column);
		}

		private bool StudyExists(string studyInstanceUid)
		{
			int foundIndex = _searchResults[_selectedServerGroup.GroupID].StudyList.Items.FindIndex(
				delegate(StudyItem test)
				{
					return test.StudyInstanceUID == studyInstanceUid;
				});

			return foundIndex >= 0;
		}

		private void AddReceivedStudies()
		{
			if (_selectedServerGroup == null || !_selectedServerGroup.IsLocalDatastore)
				return;

			if (_setStudiesArrived.Count == 0)
				return;

			List<string> newStudies = new List<string>();
			foreach (string studyUid in _setStudiesArrived.Keys)
			{
				if (!StudyExists(studyUid))
					newStudies.Add(studyUid);
			}

			if (newStudies.Count == 0)
				return;

			string studyUids = VMStringConverter.ToDicomStringArray<string>(newStudies);
			if (String.IsNullOrEmpty(studyUids))
				return;

			QueryParameters parameters = PrepareQueryParameters();
			parameters["StudyInstanceUid"] = studyUids;

			StudyItemList list = new StudyItemList();
			list = _studyFinder.Query(parameters);

			foreach (StudyItem item in list)
			{
				//don't need to check this again, it's just paranoia
				if (!StudyExists(item.StudyInstanceUID))
					_searchResults[_selectedServerGroup.GroupID].StudyList.Items.Add(item);
			}

			_setStudiesArrived.Clear();

			//update the search results title.
			_searchResults[_selectedServerGroup.GroupID].ResultsTitle = 
				String.Format(SR.FormatStudiesFound, _searchResults[_selectedServerGroup.GroupID].StudyList.Items.Count, _selectedServerGroup.Name);
		}

		private void OnSopInstanceImported(object sender, ItemEventArgs<ImportedSopInstanceInformation> e)
		{
			if (_setStudiesArrived.ContainsKey(e.Item.StudyInstanceUid))
				return;

			_setStudiesArrived[e.Item.StudyInstanceUid] = e.Item.StudyInstanceUid;
			AddReceivedStudies();

			//update the title in the view.
			this.ResultsTitle = _searchResults[_selectedServerGroup.GroupID].ResultsTitle;
		}

		private void OnConfigurationSettingsChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ShowIdeographicName" ||
				e.PropertyName == "ShowPhoneticName")
			{
				// Iterate through all the tables from all servers and turn off
				// the appropriate columns.
				foreach (SearchResult result in _searchResults.Values)
				{
					foreach (ITableColumn column in result.StudyList.Columns)
					{
						if (column.Name == SR.ColumnHeadingPhoneticName ||
							column.Name == SR.ColumnHeadingIdeographicName)
							column.Visible = DicomExplorerConfigurationSettings.Default.ShowIdeographicName;
					}
				}
			}
		}
	}
}
