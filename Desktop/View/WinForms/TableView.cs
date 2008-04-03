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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class TableView : UserControl
	{
        private event EventHandler _itemDoubleClicked;
        private event EventHandler _selectionChanged;
        private event EventHandler<ItemDragEventArgs> _itemDrag;

        private ActionModelNode _toolbarModel;
        private ActionModelNode _menuModel;

        private ToolStripItemDisplayStyle _toolStripItemDisplayStyle = ToolStripItemDisplayStyle.Image;
        private ToolStripItemAlignment _toolStripItemAlignment = ToolStripItemAlignment.Right;
        private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;

        private ITable _table;
        private bool _multiLine;

        private bool _delaySelectionChangeNotification = true; // see bug 386
        private bool _surpressSelectionChangedEvent = false;

	    private bool _isLoaded = false;

        private const int CUSTOM_CONTENT_HEIGHT = 20;
        private readonly int _rowHeight = 0;

        public TableView()
		{
			InitializeComponent();

            // if we allow the framework to generate columns, there seems to be a bug with 
            // setting the minimum column width > 100 pixels
            // therefore, turn off the auto-generate and create the columns ourselves
            _dataGridView.AutoGenerateColumns = false;

            _rowHeight = this.DataGridView.RowTemplate.Height;
            this.DataGridView.RowPrePaint += SetCustomBackground;
            this.DataGridView.RowPostPaint += DisplayCellSubRows;
            this.DataGridView.RowPostPaint += OutlineCell;
        }

        #region Design Time properties and Events

	    [DefaultValue(false)]
	    public bool SortButtonVisible
	    {
            get { return _sortButton.Visible; }
            set { _sortButton.Visible = value; }
	    }

        [DefaultValue(false)]
        public bool FilterTextBoxVisible
        {
            get { return _filterTextBox.Visible; }
            set
            {
                _filterTextBox.Visible = value;
                _clearFilterButton.Visible = value;
            }
        }

        [DefaultValue(100)]
        public int FilterTextBoxWidth
        {
            get { return _filterTextBox.Width; }
            set { _filterTextBox.Size = new Size(value, _filterTextBox.Height);; }
        }

        [DefaultValue(true)]
        public bool ReadOnly
        {
            get { return _dataGridView.ReadOnly; }
            set { _dataGridView.ReadOnly = value; }
        }

        [DefaultValue(true)]
        public bool MultiSelect
        {
            get { return _dataGridView.MultiSelect; }
            set { _dataGridView.MultiSelect = value; }
        }

        [DefaultValue(false)]
        [Description("Enables or disables multi-line rows.  If enabled, text longer than the column width is wrapped and the row is auto-sized. If disabled, a single line of truncated text is followed by an ellipsis")]
        public bool MultiLine
        {
            get { return _multiLine; }
            set
            {
                _multiLine = value;
                if (_multiLine)
                {
                    this._dataGridView.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
                    this._dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
                }
                else
                {
                    this._dataGridView.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
                    this._dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.None;
                }
            }
        }

        public ToolStripItemDisplayStyle ToolStripItemDisplayStyle
        {
            get { return _toolStripItemDisplayStyle; }
            set { _toolStripItemDisplayStyle = value; }
        }

        [DefaultValue(true)]
        public bool ShowToolbar
        {
            get { return _toolStrip.Visible; }
            set { _toolStrip.Visible = value; }
        }

        [DefaultValue(false)]
        public bool StatusBarVisible
	    {
            get { return _statusStrip.Visible; }
            set { _statusStrip.Visible = value; }
	    }

	    [DefaultValue(true)]
	    public bool ShowColumnHeading
	    {
            get { return _dataGridView.ColumnHeadersVisible; }
            set { _dataGridView.ColumnHeadersVisible = value; }
	    }

        public event EventHandler SelectionChanged
        {
            add { _selectionChanged += value; }
            remove { _selectionChanged -= value; }
        }

        public event EventHandler ItemDoubleClicked
        {
            add { _itemDoubleClicked += value; }
            remove { _itemDoubleClicked -= value; }
        }

        public event EventHandler<ItemDragEventArgs> ItemDrag
        {
            add { _itemDrag += value; }
            remove { _itemDrag -= value; }
        }

        #endregion

        #region Public Properties and Events

        [Obsolete("Do not use.  Toolstrip item alignment is now controlled by application setting")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RightToLeft ToolStripRightToLeft
        {
            get { return RightToLeft.No; }
            set { }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SuppressSelectionChangedEvent
        {
            get { return _surpressSelectionChangedEvent; }
            set { _surpressSelectionChangedEvent = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel;  }
            set
            {
                _toolbarModel = value;

                // Defer initialization of ToolStrip until after Load() has been called
                // so that parameters from application settings are initialized properly
                if (_isLoaded) InitializeToolStrip();
            }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;
                ToolStripBuilder.Clear(_contextMenu.Items);
                if (_menuModel != null)
                {
                    ToolStripBuilder.BuildMenu(_contextMenu.Items, _menuModel.ChildNodes);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string StatusText
	    {
            get { return _statusLabel.Text; }
            set { _statusLabel.Text = value; }
	    }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITable Table
        {
            get { return _table; }
            set
            {
				UnsubscribeFromOldTable();

                _table = value;

                // by setting the datasource to null here, we eliminate the SelectionChanged events that
                // would get fired during the call to InitColumns()
                _dataGridView.DataSource = null;

                InitColumns();

                if (_table != null)
                {
                    //_bindingSource.DataSource = new TableAdapter(_table);
                    //_dataGridView.DataSource = _bindingSource;
                    _dataGridView.DataSource = new TableAdapter(_table);

                    // Set a cell padding to provide space for the top of the focus 
                    // rectangle and for the content that spans multiple columns. 
                    Padding newPadding = new Padding(0, 1, 0,
                        CUSTOM_CONTENT_HEIGHT * ((int)_table.CellRowCount - 1));
                    this.DataGridView.RowTemplate.DefaultCellStyle.Padding = newPadding;

                    // Set the row height to accommodate the content that 
                    // spans multiple columns.
                    this.DataGridView.RowTemplate.Height = _rowHeight + CUSTOM_CONTENT_HEIGHT * ((int)_table.CellRowCount - 1);

                    _table.Sorted += new EventHandler(_table_SortEvent);
                }

                InitializeSortButton();
                IntializeFilter();
            }
        }

        /// <summary>
        /// Gets/sets the current selection
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISelection Selection
        {
            get
            {
                return GetSelectionHelper();
            }
            set
            {
                // if someone tries to assign null, just convert it to an empty selection - this makes everything easier
                ISelection newSelection = value ?? new Selection();

                // get the existing selection
                ISelection existingSelection = GetSelectionHelper();

                if (!existingSelection.Equals(newSelection))
                {
                    // de-select any rows that should not be selected
                    foreach (DataGridViewRow row in _dataGridView.SelectedRows)
                    {
                        if (!CollectionUtils.Contains(newSelection.Items, delegate(object item) { return item == row.DataBoundItem; }))
                        {
                            row.Selected = false;
                        }
                    }

                    // select any rows that should be selected
                    foreach (object item in newSelection.Items)
                    {
                        DataGridViewRow row = CollectionUtils.SelectFirst<DataGridViewRow>(_dataGridView.Rows,
                                delegate(DataGridViewRow r) { return r.DataBoundItem == item; });
                        if (row != null)
                            row.Selected = true;
                    }

                    EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
                }
            }
        }


        #endregion

        protected ToolStrip ToolStrip
        {
            get { return _toolStrip; }
        }

        protected new ContextMenuStrip ContextMenuStrip
        {
            get { return _contextMenu; }
        }

        private void InitializeToolStrip()
        {
            ToolStripBuilder.Clear(_toolStrip.Items);
            if (_toolbarModel != null)
            {
                if (_toolStripItemAlignment == ToolStripItemAlignment.Right)
                {
                    _toolbarModel.ChildNodes.Reverse();
                }

                ToolStripBuilder.BuildToolbar(
                    _toolStrip.Items,
                    _toolbarModel.ChildNodes,
                    new ToolStripBuilder.ToolStripBuilderStyle(_toolStripItemDisplayStyle, _toolStripItemAlignment, _textImageRelation));
            }
        }
        private Selection GetSelectionHelper()
        {
            return new Selection(
                CollectionUtils.Map<DataGridViewRow, object>(_dataGridView.SelectedRows,
                    delegate(DataGridViewRow row) { return row.DataBoundItem; }));
        }

        private void InitColumns()
        {
            // clear the old columns
            _dataGridView.Columns.Clear();

            if (_table != null)
            {
                float fontSize = this.Font.SizeInPoints;
                foreach (ITableColumn col in _table.Columns)
                {
                    // this is ugly but somebody's gotta do it
                    DataGridViewColumn dgcol;
                    if (col.ColumnType == typeof(bool))
                        dgcol = new DataGridViewCheckBoxColumn();
                    else if (col.ColumnType == typeof(Image) || col.ColumnType == typeof(IconSet))
                    {
                        dgcol = new DataGridViewImageColumn();

                        dgcol.SortMode = DataGridViewColumnSortMode.Automatic;

                        // Set the default to display nothing if not icons are provided.
                        // Otherwise WinForms will by default display an ugly icon with 'x'
                        dgcol.DefaultCellStyle.NullValue = null;
                    }
                    else
                    {
                        // assume any other type of column will be displayed as text
                        dgcol = new DataGridViewTextBoxColumn();
                    }

                    // initialize the necessary properties
                    dgcol.Name = col.Name;
                    dgcol.HeaderText = col.Name;
                    dgcol.DataPropertyName = col.Name;
                    dgcol.ReadOnly = col.ReadOnly;
                    dgcol.MinimumWidth = (int)(col.WidthFactor * _table.BaseColumnWidthChars * fontSize);
                    dgcol.FillWeight = col.WidthFactor;
					dgcol.Visible = col.Visible;

					// Associate the ITableColumn with the DataGridViewColumn
					dgcol.Tag = col;

					col.VisibleChanged += OnColumnVisibilityChanged;

                    _dataGridView.Columns.Add(dgcol);
                }
            }
        }

		private void UnsubscribeFromOldTable()
		{
			if (_table != null)
			{
				foreach (ITableColumn column in _table.Columns)
					column.VisibleChanged -= OnColumnVisibilityChanged;

			    _table.Sorted -= _table_SortEvent;
			}
		}

		private void OnColumnVisibilityChanged(object sender, EventArgs e)
		{
			// NY: Yes, I know, this is really cheap. The original plan was
			// to use anonymous delegates to "bind" the ITableColumn to the
			// DataGridViewColumn, but unsubscribing from ITableColumn.VisiblityChanged
			// was problematic.  This is the next best thing if we want
			// easy unsubscription.
            ITableColumn column = (ITableColumn)sender;  //Invalid cast is a programming error, so let exception be thrown
			DataGridViewColumn dgcolumn = FindDataGridViewColumn(column);

            if (dgcolumn != null)
                dgcolumn.Visible = column.Visible;
		}

		private DataGridViewColumn FindDataGridViewColumn(ITableColumn column)
		{
			foreach (DataGridViewColumn dgcolumn in _dataGridView.Columns)
			{
				if (dgcolumn.Tag == column)
					return dgcolumn;
			}

			return null;
		}

        // Paints the custom background for each row
        private void SetCustomBackground(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if ((e.State & DataGridViewElementStates.Selected) ==
                        DataGridViewElementStates.Selected)
            {
                // do nothing?
                return;
            }

            if (_table != null)
            {
                Rectangle rowBounds = GetAdjustedRowBounds(e.RowBounds);

                // Color.FromName("Empty") does not return Color.Empty, so need to manually check for Empty
                string colorName = _table.GetItemBackgroundColor(_table.Items[e.RowIndex]);
                Color backgroundColor = string.IsNullOrEmpty(colorName)|| colorName.Equals("Empty") ? Color.Empty : Color.FromName(colorName);

                if (backgroundColor.Equals(Color.Empty))
                {
                    backgroundColor = e.InheritedRowStyle.BackColor;
                }

                // Paint the custom selection background.
                using (Brush backbrush =
                    new SolidBrush(backgroundColor))
                {
                    e.PaintParts &= ~DataGridViewPaintParts.Background;
                    e.Graphics.FillRectangle(backbrush, rowBounds);
                }
            }
        }

        // Paints the custom outline for each row
        private void OutlineCell(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rowBounds = GetAdjustedRowBounds(e.RowBounds);

            if (_table != null)
            {
                int penWidth = 2;
                Rectangle outline = new Rectangle(
                    rowBounds.Left + penWidth / 2,
                    rowBounds.Top + penWidth / 2 + 1,
                    rowBounds.Width - penWidth,
                    rowBounds.Height - penWidth - 2);

                string colorName = _table.GetItemOutlineColor(_table.Items[e.RowIndex]);
                Color outlineColor  = string.IsNullOrEmpty(colorName) || colorName.Equals("Empty") ? Color.Empty : Color.FromName(colorName);
                using (Pen outlinePen =
                    new Pen(outlineColor, penWidth))
                {
                    e.Graphics.DrawRectangle(outlinePen, outline);
                }
            }
        }

        // Paints the content that spans multiple columns and the focus rectangle.
        private void DisplayCellSubRows(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rowBounds = GetAdjustedRowBounds(e.RowBounds);

            SolidBrush forebrush = null;
            try
            {
                // Determine the foreground color.
                if ((e.State & DataGridViewElementStates.Selected) ==
                    DataGridViewElementStates.Selected)
                {
                    forebrush = new SolidBrush(e.InheritedRowStyle.SelectionForeColor);
                }
                else
                {
                    forebrush = new SolidBrush(e.InheritedRowStyle.ForeColor);
                }

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < _table.Columns.Count; i++)
                {
                    ITableColumn col = _table.Columns[i] as ITableColumn;
                    if (col != null && col.CellRow > 0)
                    {
                        DataGridViewRow row = this.DataGridView.Rows[e.RowIndex];
                        object recipe = row.Index != -1 ? row.Cells[i].Value : null;

                        if (recipe != null)
                        {
                            sb.Append(recipe + " ");
                        }

                    }
                }

                string text = sb.ToString().Trim();

                if (string.IsNullOrEmpty(text) == false)
                {
                    // Calculate the bounds for the content that spans multiple 
                    // columns, adjusting for the horizontal scrolling position 
                    // and the current row height, and displaying only whole
                    // lines of text.
                    Rectangle textArea = rowBounds;
                    textArea.X -= this.DataGridView.HorizontalScrollingOffset;
                    textArea.Width += this.DataGridView.HorizontalScrollingOffset;
                    textArea.Y += rowBounds.Height - e.InheritedRowStyle.Padding.Bottom;
                    textArea.Height -= rowBounds.Height - e.InheritedRowStyle.Padding.Bottom;
                    textArea.Height = (textArea.Height / e.InheritedRowStyle.Font.Height) * e.InheritedRowStyle.Font.Height;

                    // Calculate the portion of the text area that needs painting.
                    RectangleF clip = textArea;
                    int startX = this.DataGridView.RowHeadersVisible ? this.DataGridView.RowHeadersWidth : 0;
                    clip.Width -= startX + 1 - clip.X;
                    clip.X = startX + 1;
                    RectangleF oldClip = e.Graphics.ClipBounds;
                    e.Graphics.SetClip(clip);

                    // Draw the content that spans multiple columns.
                    e.Graphics.DrawString(text, e.InheritedRowStyle.Font, forebrush, textArea);

                    e.Graphics.SetClip(oldClip);
                }
            }
            finally
            {
                if (forebrush != null)
                    forebrush.Dispose();
            }
        }

        private Rectangle GetAdjustedRowBounds(Rectangle rowBounds)
        {
            return new Rectangle(
                    (this.DataGridView.RowHeadersVisible ? this.DataGridView.RowHeadersWidth : 0) + rowBounds.Left,
                    rowBounds.Top,
                    this.DataGridView.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - this.DataGridView.HorizontalScrollingOffset,
                    rowBounds.Height);
        }

        private void _dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)    // rowindex == -1 represents a header click
            {
                EventsHelper.Fire(_itemDoubleClicked, this, new EventArgs());
            }
        }

        private void _contextMenu_Opening(object sender, CancelEventArgs e)
        {
            // if a context menu is being opened, need to flush any pending selection change notification immediately before
            // showing menu (bug 386)
            FlushPendingSelectionChangeNotification();
            
            // Find the row we're on
			Point pt = _dataGridView.PointToClient(DataGridView.MousePosition);
			DataGridView.HitTestInfo info = _dataGridView.HitTest(pt.X, pt.Y);


            try
            {
                // temporarily disable the delaying of selection change notifications
                // if we modify the selection while opening the context menu, we need those notifications to propagate immediately
                _delaySelectionChangeNotification = false;

                if (_dataGridView.SelectedRows.Count == 0)
                {
                    // select the new row
                    if (info.RowIndex >= 0)
                        _dataGridView.Rows[info.RowIndex].Selected = true;
                }
                else if (_dataGridView.SelectedRows.Count == 1 && _dataGridView.SelectedRows[0].Index != info.RowIndex)
                {
                    // deselect the selected row
                    _dataGridView.SelectedRows[0].Selected = false;

                    // Now select the new row
                    if (info.RowIndex >= 0)
                        _dataGridView.Rows[info.RowIndex].Selected = true;
                }
                else
                {
                    // If multiple
                    // rows are selected we don't want to deselect anything, since the
                    // user's intent is to perform a context menu operation on all
                    // selected rows.
                }

            }
            finally
            {
                // re-enable the delaying of selection change notifications
                _delaySelectionChangeNotification = true;
            }
        }

        private void _contextMenu_Opened(object sender, EventArgs e)
        {

        }

        private void _contextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {

        }

        private void _contextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {

        }

        private void _dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (_surpressSelectionChangedEvent)
                return;

            if (_delaySelectionChangeNotification)
            {
                // fix Bug 386: rather than firing our own _selectionChanged event immediately, post delayed notification
                PostSelectionChangeNotification();
            }
            else
            {
                NotifySelectionChanged();
            }
        }

        /// <summary>
        /// Handling this event is necessary to ensure that changes to checkbox cells are propagated
        /// immediately to the underlying <see cref="ITable"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // if the state of a checkbox cell has changed, commit the edit immediately
            if (_dataGridView.CurrentCell is DataGridViewCheckBoxCell)
            {
                _dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// Handle the ItemDrag event of the internal control, so that this control can fire its own 
        /// event, using the current selection as the "item" that is being dragged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dataGridView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // if a drag is being initiated, need to flush any pending selection change notification immediately before
            // proceeding (bug 386)
            FlushPendingSelectionChangeNotification();

            ItemDragEventArgs args = new ItemDragEventArgs(e.Button, this.GetSelectionHelper());
            EventsHelper.Fire(_itemDrag, this, args);
        }

        /// <summary>
        /// Fix Bug 386: Add a 50ms delay before posting selection changes to outside clients
        /// this has the effect of filtering out very quick selection changes that usually reflect
        /// "bugs" in the windowing framework rather than user actions
        /// </summary>
        private void PostSelectionChangeNotification()
        {
            // restart the timer - this effectively "posts" a selection change notification to occur on the timer tick
            // if a change notification is already pending, it is cleared
            _selectionChangeTimer.Stop();
            _selectionChangeTimer.Start();
        }

        /// <summary>
        /// If a selection change notification is pending, this method will force it to occur now rather than
        /// waiting for the timer tick. (Bug 386)
        /// </summary>
        private void FlushPendingSelectionChangeNotification()
        {
            bool pending = _selectionChangeTimer.Enabled;
            _selectionChangeTimer.Stop();   // stop the timer before firing event, in case event handler runs long duration

            // if there was a "pending" notification, send it now
            if (pending)
            {
                NotifySelectionChanged();
            }
        }

        /// <summary>
        /// Delayed selection change notification (fix for bug 386)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _selectionChangeTimer_Tick(object sender, EventArgs e)
        {
            // stop the timer (one-shot behaviour)
            _selectionChangeTimer.Stop();

            // notify clients
            NotifySelectionChanged();
        }

        private void NotifySelectionChanged()
        {
            if (_surpressSelectionChangedEvent)
                return;

            // notify clients of this class of a *real* selection change
            EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
        }


        protected DataGridView DataGridView
        {
            get { return _dataGridView; }
        }

        private void TableView_Load(object sender, EventArgs e)
        {
            if (this.DesignMode == false)
            {
                _toolStripItemAlignment = DesktopViewSettings.Default.LocalToolStripItemAlignment;
                _textImageRelation = DesktopViewSettings.Default.LocalToolStripItemTextImageRelation;
            }
            else
            {
                _toolStripItemAlignment = ToolStripItemAlignment.Left;
                _textImageRelation = TextImageRelation.ImageBeforeText;                
            }

            InitializeToolStrip();
            _isLoaded = true;
        }

        private void InitializeSortButton()
        {
            if (_table == null || _table.Columns.Count == 0)
            {
                _sortButton.Enabled = false;
            }
            else
            {
                // Rebuild dropdown menu
                _sortButton.Enabled = true;
                _sortButton.DropDownItems.Clear();
                _sortButton.DropDownItems.Add(_sortAscendingButton);
                _sortButton.DropDownItems.Add(_sortDescendingButton);
                _sortButton.DropDownItems.Add(_sortSeparator);

                CollectionUtils.ForEach<ITableColumn>(_table.Columns,
                    delegate(ITableColumn column)
                    {
                        ToolStripItem item = new ToolStripMenuItem(column.Name, null, _sortButtonDropDownItem_Click, column.Name);
                        if (_sortButton.DropDownItems.ContainsKey(column.Name) == false)
                            _sortButton.DropDownItems.Add(item);
                    });

                ResetSortButtonState();
            }
        }

        private void ResetSortButtonState()
        {
            if (_table == null || _table.SortParams == null)
                return;

            CollectionUtils.ForEach<ToolStripItem>(_sortButton.DropDownItems,
                delegate(ToolStripItem item)
                {
                    if (item == _sortAscendingButton)
                        this.SortAscendingButtonCheck = _table.SortParams.Ascending;
                    else if (item == _sortDescendingButton)
                        this.SortDescendingButtonCheck = _table.SortParams.Ascending == false;
                    else if (item == _sortSeparator)
                        return;
                    else
                    {
                        if (item.Name.Equals(_table.SortParams.Column.Name))
                        {
                            item.Image = SR.CheckSmall;
                            _sortButton.ToolTipText = String.Format(SR.MessageSortBy, item.Name);
                        }
                        else
                        {
                            item.Image = null;
                        }                        
                    }
                });
        }

	    private bool SortAscendingButtonCheck
	    {
            get { return _sortAscendingButton.Image != null; }
            set { _sortAscendingButton.Image = value ? SR.CheckSmall : null; }
	    }

        private bool SortDescendingButtonCheck
        {
            get { return _sortDescendingButton.Image != null; }
            set { _sortDescendingButton.Image = value ? SR.CheckSmall : null; }
        }

        private void _table_SortEvent(object sender, EventArgs e)
        {
            ResetSortButtonState();
        }

        private void sortAscendingButton_Click(object sender, EventArgs e)
        {
            if (_table == null || _table.SortParams == null)
                return;

            _table.SortParams.Ascending = true;
            _table.Sort(_table.SortParams);
        }

        private void sortDescendingButton_Click(object sender, EventArgs e)
        {
            if (_table == null || _table.SortParams == null)
                    return;

            _table.SortParams.Ascending = false;
            _table.Sort(_table.SortParams);
        }

        private void _sortButtonDropDownItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;

            ITableColumn sortColumn = CollectionUtils.SelectFirst<ITableColumn>(_table.Columns,
                delegate(ITableColumn column)
                {
                    return column.Name.Equals(item.Name);
                });

            if (sortColumn != null)
            {
                TableSortParams sortParams = new TableSortParams(sortColumn, false);
                _table.Sort(sortParams);
            }
        }

        private void IntializeFilter()
        {
            _filterTextBox.Enabled = (_table != null);
        }

        private void _clearFilterButton_Click(object sender, EventArgs e)
        {
            _filterTextBox.Text = "";
        }

        private void _filterText_TextChanged(object sender, EventArgs e)
        {
            if (_table == null)
                return;


            if (String.IsNullOrEmpty(_filterTextBox.Text))
            {
                _filterTextBox.ToolTipText = SR.MessageEmptyFilter;
                _clearFilterButton.Enabled = false;
                _table.RemoveFilter();
            }
            else
            {
                _filterTextBox.ToolTipText = String.Format(SR.MessageFilterBy, _filterTextBox.Text);
                _clearFilterButton.Enabled = true;
                TableFilterParams filterParams = new TableFilterParams(null, _filterTextBox.Text);
                _table.Filter(filterParams);
            }

            // Refresh the current table
            this.Table = _table;
        }
    }
}
