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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	internal class SearchResult
	{
		private string _serverGroupName;
		private bool _isLocalDataStore;
		private int _numberOfChildServers;

		private readonly Table<StudyItem> _studyTable;
		private readonly List<StudyItem> _hiddenItems;

		private bool _filterDuplicates;
		private bool _everSearched;

		public SearchResult()
		{
			_everSearched = false;
			HasDuplicates = false;

			_serverGroupName = "";
			_isLocalDataStore = false;
			_numberOfChildServers = 1;

			_hiddenItems = new List<StudyItem>();
			_studyTable = new Table<StudyItem>();

			InitializeTable();
		}

		#region Properties

		public string ServerGroupName
		{
			get { return _serverGroupName; }
			set { _serverGroupName = value; }
		}

		public bool IsLocalDataStore
		{
			get { return _isLocalDataStore; }
			set
			{
				_isLocalDataStore = value;
				UpdateServerColumnsVisibility();
			}
		}

		public int NumberOfChildServers
		{
			get { return _numberOfChildServers; }
			set
			{
				_numberOfChildServers = value;
				UpdateServerColumnsVisibility();
			}
		}

		public Table<StudyItem> StudyTable
		{
			get { return _studyTable; }
		}

		public string ResultsTitle
		{
			get
			{
				if (!_everSearched)
					return _serverGroupName;
				else
					return String.Format(SR.FormatStudiesFound, _studyTable.Items.Count, _serverGroupName);
			}
		}

		public bool HasDuplicates { get; private set; }

		public bool FilterDuplicates
		{
			get { return _filterDuplicates; }
			set
			{
				_filterDuplicates = value;
				if (_filterDuplicates)
				{
					if (_hiddenItems.Count == 0)
					{
						RemoveDuplicates(_studyTable.Items, _hiddenItems);
					}
				}
				else
				{
					if (_hiddenItems.Count > 0)
					{
						_studyTable.Items.AddRange(_hiddenItems);
						_hiddenItems.Clear();
					}
				}

				StudyTable.Sort();
			}
		}
		#endregion
		#region Methods

		public void Refresh(StudyItemList studies, bool filterDuplicates)
		{
			_everSearched = true;
			_filterDuplicates = filterDuplicates;

			_hiddenItems.Clear();
			IList<StudyItem> filteredStudies = new List<StudyItem>(studies);
			RemoveDuplicates(filteredStudies, _hiddenItems);
			HasDuplicates = _hiddenItems.Count > 0;

			if (!_filterDuplicates)
			{
				_hiddenItems.Clear();
				_studyTable.Items.Clear();
				_studyTable.Items.AddRange(studies);
			}
			else
			{
				_studyTable.Items.Clear();
				_studyTable.Items.AddRange(filteredStudies);
			}

			StudyTable.Sort();
		}

		private static void RemoveDuplicates(IList<StudyItem> allStudies, List<StudyItem> removed)
		{
			removed.Clear();

			Dictionary<string, StudyItem> uniqueStudies = new Dictionary<string, StudyItem>();
			foreach (StudyItem study in allStudies)
			{
				StudyItem existing;
				if (uniqueStudies.TryGetValue(study.StudyInstanceUid, out existing))
				{
					ApplicationEntity server = study.Server as ApplicationEntity;
					//we will only replace an existing entry if this study's server is streaming.
					if (server != null && server.IsStreaming)
					{
						//only replace existing entry if it is on a non-streaming server.
						server = existing.Server as ApplicationEntity;
						if (server == null || !server.IsStreaming)
						{
							removed.Add(existing);
							uniqueStudies[study.StudyInstanceUid] = study;
							continue;
						}
					}

					//this study is a duplicate.
					removed.Add(study);
				}
				else
				{
					uniqueStudies[study.StudyInstanceUid] = study;
				}
			}

			foreach (StudyItem study in removed)
				allStudies.Remove(study);
		}

		private void InitializeTable()
		{
			TableColumn<StudyItem, string> column;

			try
			{
				// Create and add any extension columns
				StudyColumnExtensionPoint xp = new StudyColumnExtensionPoint();
				foreach (object obj in xp.CreateExtensions())
				{
					IStudyColumn newColumn = (IStudyColumn)obj;

					column = new TableColumn<StudyItem, string>(
						newColumn.Name,
						item => (newColumn.GetValue(item) ?? "").ToString(),
						newColumn.WidthFactor);

					newColumn.ColumnValueChanged += OnColumnValueChanged;
					_studyTable.Columns.Add(column);
				}
			}
			catch (NotSupportedException)
			{
			}

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingPatientId,
				delegate(StudyItem item) { return item.PatientId; },
				0.5f);

			_studyTable.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingLastName,
				delegate(StudyItem item) { return item.PatientsName.LastName; },
				0.5f);

			_studyTable.Columns.Add(column);

			// Default: Sort by lastname
			_studyTable.Sort(new TableSortParams(column, true));

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingFirstName,
				delegate(StudyItem item) { return item.PatientsName.FirstName; },
				0.5f);

			_studyTable.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingIdeographicName,
				delegate(StudyItem item) { return item.PatientsName.Ideographic; },
				0.5f);

			column.Visible = DicomExplorerConfigurationSettings.Default.ShowIdeographicName;

			_studyTable.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingPhoneticName,
				delegate(StudyItem item) { return item.PatientsName.Phonetic; },
				0.5f);

			column.Visible = DicomExplorerConfigurationSettings.Default.ShowPhoneticName;

			_studyTable.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingDateOfBirth,
				delegate(StudyItem item) { return GetDateStringFromDicomDA(item.PatientsBirthDate); },
				null,
				0.4F,
				delegate(StudyItem one, StudyItem two) { return one.PatientsBirthDate.CompareTo(two.PatientsBirthDate); });

			_studyTable.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingAccessionNumber,
				delegate(StudyItem item) { return item.AccessionNumber; },
				0.45F);

			_studyTable.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingStudyDate,
				delegate(StudyItem item) { return GetDateStringFromDicomDA(item.StudyDate); },
				null,
				0.4F,
				delegate(StudyItem one, StudyItem two) { return one.StudyDate.CompareTo(two.StudyDate); });

			_studyTable.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingStudyDescription,
				delegate(StudyItem item) { return item.StudyDescription; },
				0.75F);

			_studyTable.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingModality,
				delegate(StudyItem item) { return DicomStringHelper.GetDicomStringArray(item.ModalitiesInStudy); },
				0.25f);

			_studyTable.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingReferringPhysician,
				delegate(StudyItem item)
					{
						if (item.ReferringPhysiciansName != null)
							return item.ReferringPhysiciansName.FormattedName;
						else
							return "";
					},
				0.6f);

			_studyTable.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingNumberOfInstances,
				delegate(StudyItem item)
					{
						if (item.NumberOfStudyRelatedInstances.HasValue)
							return item.NumberOfStudyRelatedInstances.ToString();
						else
							return "";
					},
				null,
				0.3f,
					delegate(StudyItem study1, StudyItem study2)
				                      	         	{
				                      	         		int? instances1 = study1.NumberOfStudyRelatedInstances;
														int? instances2 = study2.NumberOfStudyRelatedInstances;

														if (instances1 == null)
				                      	         		{
															if (instances2 == null)
																return 0;
															else
																return 1;
				                      	         		}
														else if (instances2 == null)
				                      	         		{
				                      	         			return -1;
				                      	         		}

				                      	         		return -instances1.Value.CompareTo(instances2.Value);
													});

			column.Visible = DicomExplorerConfigurationSettings.Default.ShowNumberOfImagesInStudy;

			_studyTable.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(SR.ColumnHeadingServer,
														delegate(StudyItem item)
														{
															return (item.Server == null) ? "" : item.Server.ToString();
														},
														0.3f);

			_studyTable.Columns.Add(column);

			column = new TableColumn<StudyItem, string>(SR.ColumnHeadingAvailability,
														delegate(StudyItem item)
														{
															return item.InstanceAvailability ?? "";
														},
														0.3f);

			_studyTable.Columns.Add(column);
		}

		public void UpdateColumnVisibility()
		{
			foreach (TableColumnBase<StudyItem> column in _studyTable.Columns)
			{
				if (column.Name == SR.ColumnHeadingPhoneticName ||
					column.Name == SR.ColumnHeadingIdeographicName)
				{
					column.Visible = DicomExplorerConfigurationSettings.Default.ShowIdeographicName;
				}
				else if (column.Name == SR.ColumnHeadingNumberOfInstances)
				{
					column.Visible = DicomExplorerConfigurationSettings.Default.ShowNumberOfImagesInStudy;
				}
			}

			UpdateServerColumnsVisibility();
		}

		private void UpdateServerColumnsVisibility()
		{
			TableColumnBase<StudyItem> column = FindColumn(SR.ColumnHeadingServer);
			if (column != null)
			{
				if (_isLocalDataStore || _numberOfChildServers == 1)
					column.Visible = false;
				else
					column.Visible = true;
			}

			column = FindColumn(SR.ColumnHeadingAvailability);
			if (column != null)
			{
				if (_isLocalDataStore)
					column.Visible = false;
				else
					column.Visible = true;
			}
		}

		private TableColumnBase<StudyItem> FindColumn(string columnHeading)
		{
			foreach (TableColumnBase<StudyItem> column in StudyTable.Columns)
			{
				if (column.Name == columnHeading)
					return column;
			}

			return null;
		}

		private void OnColumnValueChanged(object sender, ItemEventArgs<StudyItem> e)
		{
			this.StudyTable.Items.NotifyItemUpdated(e.Item);
		}

		#endregion

		private static string GetDateStringFromDicomDA(string dicomDate)
		{
			DateTime date;
			if (!DateParser.Parse(dicomDate, out date))
				return dicomDate;

			return date.ToString(Format.DateFormat);
		}
	}
}
