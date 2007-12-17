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
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	[ExtensionPoint]
	public sealed class PresetVoiLutConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(PresetVoiLutConfigurationComponentViewExtensionPoint))]
	public sealed class PresetVoiLutConfigurationComponent : ConfigurationApplicationComponent
	{
		public sealed class PresetFactoryDescriptor : IEquatable<PresetFactoryDescriptor>, IComparable<PresetFactoryDescriptor>
		{
			internal readonly IPresetVoiLutOperationFactory Factory;

			internal PresetFactoryDescriptor(IPresetVoiLutOperationFactory factory)
			{
				Platform.CheckForNullReference(factory, "factory");
				Factory = factory;
			}

			public string Name
			{
				get { return Factory.Name; }
			}

			public string Description
			{
				get { return Factory.Description; }
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is PresetFactoryDescriptor)
					return this.Equals((PresetFactoryDescriptor) obj);

				return false;
			}

			public override string ToString()
			{
				return this.Factory.Description;
			}

			#region IEquatable<PresetFactoryDescriptor> Members

			public bool Equals(PresetFactoryDescriptor other)
			{
				if (other == null)
					return false;

				return this.Factory == other.Factory;
			}

			#endregion

			#region IComparable<PresetFactoryDescriptor> Members

			public int CompareTo(PresetFactoryDescriptor other)
			{
				return this.Factory.Description.CompareTo(other.Factory.Description);
			}

			#endregion
		}

		private List<PresetFactoryDescriptor> _availableAddFactories;
		private PresetFactoryDescriptor _selectedAddFactory;

		private List<string> _availableModalities;
		private PresetVoiLutGroupCollection _presetVoiLutGroups;
		private PresetVoiLutGroup _selectedPresetVoiLutGroup;

		private ITable _voiLutPresets;
		private ISelection _selection;

		private SimpleActionModel _toolbarModel;
		private SimpleActionModel _contextMenuModel;

		public PresetVoiLutConfigurationComponent()
		{
		}

		public IList<PresetFactoryDescriptor> AvailableAddFactories
		{
			get { return _availableAddFactories; }	
		}

		public PresetFactoryDescriptor SelectedAddFactory
		{
			get { return _selectedAddFactory; }	
			set
			{
				Platform.CheckForNullReference(value, "value");

				if (value.Equals(_selectedAddFactory))
					return;

				_selectedAddFactory = value;
				UpdateButtonStates();
				NotifyPropertyChanged("SelectedAddFactory");
			}
		}

		public IList<string> Modalities
		{
			get { return _availableModalities; }
		}

		public string SelectedModality
		{
			get { return _selectedPresetVoiLutGroup.Modality; }
			set
			{
				if (_selectedPresetVoiLutGroup.Modality == value)
					return;

				foreach(PresetVoiLutGroup group in _presetVoiLutGroups)
				{
					if (group.Modality == value)
					{
						SynchronizeCurrentSelectedGroup();
						_selectedPresetVoiLutGroup = group;
						PopulateTable();
						base.NotifyPropertyChanged("SelectedModality");
						return;
					}
				}

				throw new ArgumentException(SR.ExceptionInputValueDoesntMatchExistingModality);
			}
		}

		public ITable VoiLutPresets
		{
			get { return _voiLutPresets; }
		}

		public ISelection Selection
		{
			get { return _selection; }
			set
			{
				_selection = value;
				NotifyPropertyChanged("Selection");

				UpdateButtonStates();
			}
		}

		public ActionModelNode ToolbarModel
		{
			get
			{
				return _toolbarModel;
			}
		}

		public ActionModelNode ContextMenuModel
		{
			get
			{
				return _contextMenuModel;
			}
		}

		public bool HasMultipleFactories
		{
			get
			{
				return _availableAddFactories.Count > 1;
			}
		}

		public bool AddEnabled
		{
			get
			{
				return _toolbarModel["add"].Enabled;
			}
			private set
			{
				_toolbarModel["add"].Enabled = value;
				_contextMenuModel["add"].Enabled = value;
				NotifyPropertyChanged("AddEnabled");
			}
		}

		public bool EditEnabled
		{
			get { return _toolbarModel["edit"].Enabled; }
			private set
			{
				_toolbarModel["edit"].Enabled = value;
				_contextMenuModel["edit"].Enabled = value;
				NotifyPropertyChanged("EditEnabled");
			}
		}

		private bool DeleteEnabled
		{
			get { return _toolbarModel["delete"].Enabled; }
			set
			{
				_toolbarModel["delete"].Enabled = value;
				_contextMenuModel["delete"].Enabled = value;
				NotifyPropertyChanged("DeleteEnabled");
			}
		}

		private PresetVoiLut SelectedPresetVoiLut
		{
			get
			{
				if (this.Selection == null)
					return null;

				return (PresetVoiLut)this.Selection.Item;
			}
		}

		private IPresetVoiLutOperation SelectedPresetOperation
		{
			get
			{
				if (this.SelectedPresetVoiLut == null)
					return null;

				return this.SelectedPresetVoiLut.Operation;
			}
		}

		public override void Start()
		{
			InitializeAddFactories();
			_selectedAddFactory = new PresetFactoryDescriptor(PresetVoiLutOperationFactories.GetFactory(LinearPresetVoiLutOperationFactory.FactoryName));

			_voiLutPresets = new Table<PresetVoiLut>();

			_availableModalities = new List<string>(StandardModalities.Modalities);
			_availableModalities.Sort();

			_presetVoiLutGroups = PresetVoiLutSettings.Default.GetPresetGroups().Clone();

			foreach (string modality in _availableModalities)
			{
				if (!_presetVoiLutGroups.Contains(new PresetVoiLutGroup(modality)))
					_presetVoiLutGroups.Add(new PresetVoiLutGroup(modality));
			}

			_presetVoiLutGroups.Sort();

			_selectedPresetVoiLutGroup = _presetVoiLutGroups[0];

			InitializeMenuAndToolbar();
			InitializeTable();
			PopulateTable();

			Selection = null;

			base.Start();
		}

		public void OnAdd()
		{
			if (!AddEnabled)
				return;

			try
			{
				IPresetVoiLutOperationComponent addComponent = SelectedAddFactory.Factory.GetEditComponent(null);
				addComponent.EditContext = EditContext.Add;

				PresetVoiLutOperationsComponentContainer container = 
					new PresetVoiLutOperationsComponentContainer(GetUnusedKeyStrokes(null), addComponent);

				if (ApplicationComponentExitCode.Accepted != ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, container, SR.TitleAddPreset))
					return;

				PresetVoiLut preset = container.GetPresetVoiLut();
				Platform.CheckForNullReference(preset, "preset");

				List<PresetVoiLut> conflictingItems = CollectionUtils.Select<PresetVoiLut>(_voiLutPresets.Items,
				                                                                           delegate(PresetVoiLut test){ return preset.Equals(test); });

				if (conflictingItems.Count == 0)
				{
					_voiLutPresets.Items.Add(preset);
					Selection = null;

					this.Modified = true;
				}
				else
				{
					this.Host.DesktopWindow.ShowMessageBox(SR.MessageNameOrKeystrokeConflictsWithExistingPreset, MessageBoxActions.Ok);
				}
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToAddPreset, this.Host.DesktopWindow);
			}
		}

		public void OnEditSelected()
		{
			if (!EditEnabled)
				return;

			if (this.SelectedPresetVoiLut == null)
			{
				this.Host.DesktopWindow.ShowMessageBox(SR.MessagePleaseSelectAnItemToEdit, MessageBoxActions.Ok);
				return;
			}

			try
			{
				PresetVoiLutConfiguration configuration = this.SelectedPresetOperation.GetConfiguration();
				IPresetVoiLutOperationComponent editComponent = this.SelectedPresetOperation.SourceFactory.GetEditComponent(configuration);
				editComponent.EditContext = EditContext.Edit;
				PresetVoiLutOperationsComponentContainer container = new PresetVoiLutOperationsComponentContainer(GetUnusedKeyStrokes(this.SelectedPresetVoiLut), editComponent);
				container.SelectedKeyStroke = this.SelectedPresetVoiLut.KeyStroke;

				if (ApplicationComponentExitCode.Accepted != ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, container, SR.TitleEditPreset))
					return;

				PresetVoiLut preset = container.GetPresetVoiLut();
				Platform.CheckForNullReference(preset, "preset");

				List<PresetVoiLut> conflictingItems = CollectionUtils.Select<PresetVoiLut>(_voiLutPresets.Items,
				                                                                           delegate(PresetVoiLut test){ return preset.Equals(test); });

				if (conflictingItems.Count == 0 ||
				    (conflictingItems.Count == 1 && conflictingItems[0].Equals(this.SelectedPresetVoiLut)))
				{
					PresetVoiLut selected = this.SelectedPresetVoiLut;

					int index = _voiLutPresets.Items.IndexOf(selected);
					_voiLutPresets.Items.Remove(selected);

					if (index < _voiLutPresets.Items.Count)
						_voiLutPresets.Items.Insert(index, preset);
					else
						_voiLutPresets.Items.Add(preset);

					Selection = null;

					this.Modified = true;
				}
				else
				{
					this.Host.DesktopWindow.ShowMessageBox(SR.MessageNameOrKeystrokeConflictsWithExistingPreset, MessageBoxActions.Ok);
				}
			}
			catch(Exception	e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToEditPreset, this.Host.DesktopWindow);
			}
		}

		public void OnDeleteSelected()
		{
			if (!DeleteEnabled)
				return;

			if (this.SelectedPresetVoiLut == null)
			{
				this.Host.DesktopWindow.ShowMessageBox(SR.MessagePleaseSelectAnItemToDelete, MessageBoxActions.Ok);
				return;
			}

			_voiLutPresets.Items.Remove(this.SelectedPresetVoiLut);
			this.Selection = null;

			this.Modified = true;
		}

		public override void Save()
		{
			try
			{
				SynchronizeCurrentSelectedGroup();
				PresetVoiLutSettings.Default.SetPresetGroups(_presetVoiLutGroups);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToSaveWindowLevelPresetChanges, this.Host.DesktopWindow);
			}
		}

		private void InitializeMenuAndToolbar()
		{
			ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

			_toolbarModel = new SimpleActionModel(resolver);
			_toolbarModel.AddAction("add", "Add", "AddToolSmall.png", OnAdd);
			_toolbarModel.AddAction("edit", "Edit", "EditToolSmall.png", OnEditSelected);
			_toolbarModel.AddAction("delete", "Delete", "DeleteToolSmall.png", OnDeleteSelected);

			_contextMenuModel = new SimpleActionModel(resolver);
			_contextMenuModel.AddAction("add", "Add", "AddToolSmall.png", OnAdd);
			_contextMenuModel.AddAction("edit", "Edit", "EditToolSmall.png", OnEditSelected);
			_contextMenuModel.AddAction("delete", "Delete", "DeleteToolSmall.png", OnDeleteSelected);

			_toolbarModel["add"].Visible = !HasMultipleFactories;
			_contextMenuModel["add"].Visible = !HasMultipleFactories;

			UpdateButtonStates();
		}

		private void InitializeTable()
		{
			TableColumn<PresetVoiLut, string> column;

			column = new TableColumn<PresetVoiLut, string>("Key", delegate(PresetVoiLut item) { return item.KeyStrokeDescriptor.ToString(); }, 0.2f);
			_voiLutPresets.Columns.Add(column);
	
			column = new TableColumn<PresetVoiLut, string>("Name", delegate(PresetVoiLut item) { return item.Operation.Name; }, 0.2f);
			_voiLutPresets.Columns.Add(column);

			column = new TableColumn<PresetVoiLut, string>("Description", delegate(PresetVoiLut item) { return item.Operation.Description; }, 0.6f);
			_voiLutPresets.Columns.Add(column);
		}

		private void PopulateTable()
		{
			this.Selection = null;

			_voiLutPresets.Items.Clear();

			foreach (PresetVoiLut preset in _selectedPresetVoiLutGroup.Presets)
				_voiLutPresets.Items.Add(preset);
		}
		
		private List<XKeys> GetUnusedKeyStrokes(PresetVoiLut include)
		{
			List<XKeys> keyStrokes = new List<XKeys>(AvailablePresetVoiLutKeyStrokeSettings.Default.GetAvailableKeyStrokes());

			foreach (PresetVoiLut presetVoiLut in _voiLutPresets.Items)
			{
				if (include != null && include.KeyStroke == presetVoiLut.KeyStroke)
					continue;

				keyStrokes.Remove(presetVoiLut.KeyStroke);
			}

			if (!keyStrokes.Contains(XKeys.None))
				keyStrokes.Add(XKeys.None);

			//put 'None' at the top.
			keyStrokes.Sort();

			return keyStrokes;
		}

		private void InitializeAddFactories()
		{
			if (_availableAddFactories == null)
				_availableAddFactories = new List<PresetFactoryDescriptor>();

			foreach (IPresetVoiLutOperationFactory factory in PresetVoiLutOperationFactories.Factories)
				_availableAddFactories.Add(new PresetFactoryDescriptor(factory));

			_availableAddFactories.Sort();
		}

		private void UpdateButtonStates()
		{
			bool editDeleteEnabled = this.Selection != null && this.Selection.Item != null;
			//update the edit & delete toolbar and context menu buttons.
			EditEnabled = editDeleteEnabled;
			DeleteEnabled = editDeleteEnabled;

			bool addEnabled = true;
			if (!_selectedAddFactory.Factory.CanCreateMultiple)
			{
				foreach (PresetVoiLut preset in _voiLutPresets.Items)
				{
					if (preset.Operation.SourceFactory == _selectedAddFactory.Factory)
					{
						addEnabled = false;
						break;
					}
				}
			}

			AddEnabled = addEnabled;
		}

		private void SynchronizeCurrentSelectedGroup()
		{
			_selectedPresetVoiLutGroup.Presets.Clear();
			foreach (PresetVoiLut preset in _voiLutPresets.Items)
			{
				_selectedPresetVoiLutGroup.Presets.Add(preset);
			}
		}
	}
}