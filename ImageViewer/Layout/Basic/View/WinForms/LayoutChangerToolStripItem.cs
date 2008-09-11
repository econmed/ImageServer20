using System;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
	/// A custom <see cref="ToolStripItem"/> control that hosts a <see cref="TableDimensionsPicker"/>.
	/// </summary>
	internal class LayoutChangerToolStripItem : ToolStripControlHost
	{
		private readonly Color CLEARCANVAS_BLUE = Color.FromArgb(0, 164, 228);
		private readonly TableDimensionsPicker _picker;
		private readonly LayoutChangerAction _action;
		private readonly Label _label;
		private readonly Panel _panel;
		private readonly Panel _content;
		private readonly Size _defaultSize;
		private ToolStrip _owner;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="action">The action to which this view is bound.</param>
		public LayoutChangerToolStripItem(LayoutChangerAction action) : base(new Panel())
		{
			const int cellWidth = 25;
			int idealPickerWidth = (action.MaxColumns - 1) * cellWidth * 6 / 5 + cellWidth;
			int idealPickerHeight = (action.MaxRows - 1) * cellWidth * 6 / 5 + cellWidth;

			_action = action;

			_picker = new TableDimensionsPicker(_action.MaxRows, _action.MaxColumns);
			_picker.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			_picker.BackColor = Color.Transparent;
			_picker.CellSpacing = new TableDimensionsCellSpacing(cellWidth/5, cellWidth/5);
			_picker.CellStyle = new TableDimensionsCellStyle(Color.FromArgb(0, 71, 98), 1);
			_picker.HotCellStyle = new TableDimensionsCellStyle(CLEARCANVAS_BLUE, 1);
			_picker.SelectedCellStyle = new TableDimensionsCellStyle();
			_picker.Size = new Size(idealPickerWidth, idealPickerHeight);
			_picker.DimensionsSelected += OnDimensionsSelected;
			_picker.HotDimensionsChanged += OnHotTrackingDimensionsChanged;
			_label = new Label();
			_label.AutoSize = false;
			_label.BackColor = Color.Transparent;
			_label.Click += OnCancel;
			_label.Dock = DockStyle.Top;
			_label.Size = new Size(idealPickerWidth, 21);
			_label.Text = _action.Label;
			_label.TextAlign = ContentAlignment.MiddleCenter;
			_content = new Panel();
			_content.Dock = DockStyle.Fill;
			_content.Size = new Size(idealPickerWidth, idealPickerHeight);
			_content.Resize += OnContentPanelResize;
			_content.Controls.Add(_picker);
			_panel = (Panel) base.Control;
			_panel.Size = _defaultSize = new Size(Math.Max(base.Width, idealPickerWidth), idealPickerHeight + _label.Height);
			_panel.Controls.Add(_content);
			_panel.Controls.Add(_label);

			base.AutoSize = false;
			base.BackColor = Color.Transparent;
			base.ControlAlign = ContentAlignment.TopCenter;
			this.MyOwner = base.Owner;
			base.Size = _defaultSize = new Size(Math.Max(base.Width, idealPickerWidth), idealPickerHeight + _label.Height);
		}

		~LayoutChangerToolStripItem()
		{
			this.MyOwner = null;
			_picker.DimensionsSelected -= OnDimensionsSelected;
			_picker.HotDimensionsChanged -= OnHotTrackingDimensionsChanged;
			_label.Click -= OnCancel;
		}

		protected override Size DefaultSize
		{
			get { return _defaultSize; }
		}

		public override Size GetPreferredSize(Size constrainingSize)
		{
			return _defaultSize;
		}

		protected override bool DismissWhenClicked
		{
			get { return true; }
		}

		private void OnContentPanelResize(object sender, EventArgs e)
		{
			_picker.Location = new Point((_content.Width - _picker.Width)/2, 0);
		}

		/// <summary>
		/// Fired when the hot-tracked cell changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnHotTrackingDimensionsChanged(object sender, EventArgs e)
		{
			if (_picker.HotDimensions.IsEmpty)
				_label.Text = _action.Label;
			else
				_label.Text = string.Format(SR.FormatRowsColumns, _picker.HotDimensions.Height, _picker.HotDimensions.Width);
		}

		/// <summary>
		/// Fired when the user selects a layout.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDimensionsSelected(object sender, TableDimensionsEventArgs e)
		{
			_action.SetLayout(e.Rows, e.Columns);
			CloseDropDown();
		}

		/// <summary>
		/// Fired when the user clicks on the cancel label bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancel(object sender, EventArgs e)
		{
			CloseDropDown();
		}

		/// <summary>
		/// Closes the dropdown, if this ToolStripItem is on a dropdown.
		/// </summary>
		private void CloseDropDown()
		{
			if (base.IsOnDropDown)
			{
				base.PerformClick();
			}
		}

		/// <remarks>
		/// Yes, this is an incredibly convoluted way to determine max width of toolstripitems in the same menu at runtime
		/// However, it is the only one that seems to work.
		/// </remarks>
		private ToolStrip MyOwner {
			get { return _owner; }
			set {
				if (_owner != value) {
					if (_owner != null)
						_owner.Resize -= OnOwnerResize;

					_owner = value;

					if (_owner != null)
						_owner.Resize += OnOwnerResize;
				}
			}
		}

		protected override void OnOwnerChanged(EventArgs e) {
			this.MyOwner = base.Owner;
			base.OnOwnerChanged(e);
		}

		private void OnOwnerResize(object sender, EventArgs e) {
			int maxWidth = _panel.Width;
			foreach (ToolStripItem item in this.MyOwner.Items) {
				maxWidth = Math.Max(item.Width, maxWidth);
			}
			_panel.Size = new Size(maxWidth, _panel.Height);
		}
	}
}