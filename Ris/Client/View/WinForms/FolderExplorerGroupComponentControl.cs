using System;
using System.ComponentModel;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="FolderExplorerGroupComponent"/>
    /// </summary>
	public partial class FolderExplorerGroupComponentControl : ApplicationComponentUserControl
    {
        private readonly FolderExplorerGroupComponent _component;
		private bool _isLoaded = false;


        /// <summary>
        /// Constructor
        /// </summary>
        public FolderExplorerGroupComponentControl(FolderExplorerGroupComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

			_component.SelectedFolderSystemChanged += delegate
				{
					InitializeToolStrip();
				};

			Control stackTabGroups = (Control)_component.StackTabComponentContainerHost.ComponentView.GuiElement;
			stackTabGroups.Dock = DockStyle.Fill;
			_groupPanel.Controls.Add(stackTabGroups);

			this.DataBindings.Add("SearchTextBoxEnabled", _component, "SearchEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			this.DataBindings.Add("SearchTextBoxMessage", _component, "SearchMessage", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		#region Properties

    	// used by databinding within this control only
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SearchTextBoxEnabled
		{
			get { return _searchTextBox.Enabled; }
			set { _searchTextBox.Enabled = value; }
		}

		// used by databinding within this control only
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string SearchTextBoxMessage
		{
			get { return _searchTextBox.ToolTipText; }
			set { _searchTextBox.ToolTipText = value; }
		}

		#endregion

		#region Event Handlers

		private void FolderExplorerGroupComponentControl_Load(object sender, EventArgs e)
		{
			InitializeToolStrip();

			_isLoaded = true;
		}

		private void _searchButton_Click(object sender, EventArgs e)
		{
			_component.Search(new SearchData(_searchTextBox.Text, false));
		}

		private void _searchTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				_component.Search(new SearchData(_searchTextBox.Text, false));
		}

		private void _searchTextBox_TextChanged(object sender, EventArgs e)
		{
			_searchButton.Enabled = _searchTextBox.Enabled && !string.IsNullOrEmpty(_searchTextBox.Text);
		}

		private void _searchTextBox_EnabledChanged(object sender, EventArgs e)
		{
			_searchButton.Enabled = _searchTextBox.Enabled && !string.IsNullOrEmpty(_searchTextBox.Text);
		}

		#endregion

		private void InitializeToolStrip()
		{
			ToolStripBuilder.Clear(_toolStrip.Items);
			if (_component.ToolbarModel != null)
			{
				ToolStripBuilder.BuildToolbar(_toolStrip.Items, _component.ToolbarModel.ChildNodes);
			}
		}
	}
}
