using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
    /// Provides the user-interface for <see cref="LayoutComponentView"/>
	/// </summary>
	public class LayoutControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label _tileColumnsLabel;
		private System.Windows.Forms.Label _tileRowsLabel;
		private System.Windows.Forms.Label _imageBoxColumnsLabel;
		private System.Windows.Forms.Label _imageBoxRowsLabel;
		private ClearCanvas.Controls.WinForms.NonEmptyNumericUpDown _imageBoxRows;
		private ClearCanvas.Controls.WinForms.NonEmptyNumericUpDown _imageBoxColumns;
		private ClearCanvas.Controls.WinForms.NonEmptyNumericUpDown _tileColumns;
		private ClearCanvas.Controls.WinForms.NonEmptyNumericUpDown _tileRows;
		private Button _applyTiles;
		private Button _applyImageBoxes;
		private Panel imageBoxPanel;
		private Panel tilePanel;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


        private LayoutComponent _layoutComponent;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
        private BindingSource _bindingSource;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="component">The component to look at</param>
		public LayoutControl(LayoutComponent component)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            _layoutComponent = component;

            // rather than binding directly to the component, create a binding source
            // this is the only way that we can pull data from the component on demand
            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _layoutComponent;

            // bind control values
            _tileColumns.DataBindings.Add("Value", _bindingSource, "TileColumns", true, DataSourceUpdateMode.OnPropertyChanged);
            _tileRows.DataBindings.Add("Value", _bindingSource, "TileRows", true, DataSourceUpdateMode.OnPropertyChanged);
            _imageBoxColumns.DataBindings.Add("Value", _bindingSource, "ImageBoxColumns", true, DataSourceUpdateMode.OnPropertyChanged);
            _imageBoxRows.DataBindings.Add("Value", _bindingSource, "ImageBoxRows", true, DataSourceUpdateMode.OnPropertyChanged);

            // bind control enablement
            _imageBoxColumns.DataBindings.Add("Enabled", _bindingSource, "ImageBoxSectionEnabled");
            _imageBoxRows.DataBindings.Add("Enabled", _bindingSource, "ImageBoxSectionEnabled");
            _applyImageBoxes.DataBindings.Add("Enabled", _bindingSource, "ImageBoxSectionEnabled");

            _tileColumns.DataBindings.Add("Enabled", _bindingSource, "TileSectionEnabled");
            _tileRows.DataBindings.Add("Enabled", _bindingSource, "TileSectionEnabled");
            _applyTiles.DataBindings.Add("Enabled", _bindingSource, "TileSectionEnabled");


            // listen for changes to the layout subject
            _layoutComponent.SubjectChanged += new EventHandler(LayoutSubjectChangedEventHandler);
     
        }

        /// <summary>
        /// Event handler for the <see cref="LayoutComponent.LayoutSubjectChanged"/> event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutSubjectChangedEventHandler(object sender, EventArgs e)
        {
            // the subject changed, so all the data needs to be refreshed from the component
            _bindingSource.ResetBindings(false);
        }

        /// <summary>
        /// Event handler for the image boxes Apply button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _applyImageBoxes_Click(object sender, EventArgs e)
        {
            _layoutComponent.ApplyImageBoxLayout();
        }

        /// <summary>
        /// Event handler for the tiles Apply button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _applyTiles_Click(object sender, EventArgs e)
        {
            _layoutComponent.ApplyTileLayout();
        }

        /// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._tileColumnsLabel = new System.Windows.Forms.Label();
			this._tileRowsLabel = new System.Windows.Forms.Label();
			this._imageBoxColumnsLabel = new System.Windows.Forms.Label();
			this._imageBoxRowsLabel = new System.Windows.Forms.Label();
			this._applyTiles = new System.Windows.Forms.Button();
			this._applyImageBoxes = new System.Windows.Forms.Button();
			this.imageBoxPanel = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._imageBoxColumns = new ClearCanvas.Controls.WinForms.NonEmptyNumericUpDown();
			this._imageBoxRows = new ClearCanvas.Controls.WinForms.NonEmptyNumericUpDown();
			this.tilePanel = new System.Windows.Forms.Panel();
			this._tileColumns = new ClearCanvas.Controls.WinForms.NonEmptyNumericUpDown();
			this._tileRows = new ClearCanvas.Controls.WinForms.NonEmptyNumericUpDown();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.imageBoxPanel.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).BeginInit();
			this.tilePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._tileColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._tileRows)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tileColumnsLabel
			// 
			this._tileColumnsLabel.Location = new System.Drawing.Point(112, 28);
			this._tileColumnsLabel.Name = "_tileColumnsLabel";
			this._tileColumnsLabel.Size = new System.Drawing.Size(70, 23);
			this._tileColumnsLabel.TabIndex = 7;
			this._tileColumnsLabel.Text = "Columns";
			// 
			// _tileRowsLabel
			// 
			this._tileRowsLabel.Location = new System.Drawing.Point(27, 28);
			this._tileRowsLabel.Name = "_tileRowsLabel";
			this._tileRowsLabel.Size = new System.Drawing.Size(48, 23);
			this._tileRowsLabel.TabIndex = 6;
			this._tileRowsLabel.Text = "Rows";
			// 
			// _imageBoxColumnsLabel
			// 
			this._imageBoxColumnsLabel.Location = new System.Drawing.Point(112, 32);
			this._imageBoxColumnsLabel.Name = "_imageBoxColumnsLabel";
			this._imageBoxColumnsLabel.Size = new System.Drawing.Size(67, 23);
			this._imageBoxColumnsLabel.TabIndex = 2;
			this._imageBoxColumnsLabel.Text = "Columns";
			// 
			// _imageBoxRowsLabel
			// 
			this._imageBoxRowsLabel.Location = new System.Drawing.Point(27, 31);
			this._imageBoxRowsLabel.Name = "_imageBoxRowsLabel";
			this._imageBoxRowsLabel.Size = new System.Drawing.Size(48, 23);
			this._imageBoxRowsLabel.TabIndex = 1;
			this._imageBoxRowsLabel.Text = "Rows";
			// 
			// _applyTiles
			// 
			this._applyTiles.Location = new System.Drawing.Point(59, 97);
			this._applyTiles.Name = "_applyTiles";
			this._applyTiles.Size = new System.Drawing.Size(75, 23);
			this._applyTiles.TabIndex = 10;
			this._applyTiles.Text = "Apply";
			this._applyTiles.Click += new System.EventHandler(this._applyTiles_Click);
			// 
			// _applyImageBoxes
			// 
			this._applyImageBoxes.Location = new System.Drawing.Point(59, 98);
			this._applyImageBoxes.Name = "_applyImageBoxes";
			this._applyImageBoxes.Size = new System.Drawing.Size(75, 23);
			this._applyImageBoxes.TabIndex = 5;
			this._applyImageBoxes.Text = "Apply";
			this._applyImageBoxes.Click += new System.EventHandler(this._applyImageBoxes_Click);
			// 
			// imageBoxPanel
			// 
			this.imageBoxPanel.Controls.Add(this.groupBox1);
			this.imageBoxPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.imageBoxPanel.Location = new System.Drawing.Point(0, 0);
			this.imageBoxPanel.Name = "imageBoxPanel";
			this.imageBoxPanel.Size = new System.Drawing.Size(225, 155);
			this.imageBoxPanel.TabIndex = 19;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this._imageBoxColumnsLabel);
			this.groupBox1.Controls.Add(this._imageBoxColumns);
			this.groupBox1.Controls.Add(this._imageBoxRows);
			this.groupBox1.Controls.Add(this._imageBoxRowsLabel);
			this.groupBox1.Controls.Add(this._applyImageBoxes);
			this.groupBox1.Location = new System.Drawing.Point(15, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(193, 152);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Image Box";
			// 
			// _imageBoxColumns
			// 
			this._imageBoxColumns.Font = new System.Drawing.Font("Arial", 9.75F);
			this._imageBoxColumns.Location = new System.Drawing.Point(115, 58);
			this._imageBoxColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._imageBoxColumns.Name = "_imageBoxColumns";
			this._imageBoxColumns.Size = new System.Drawing.Size(48, 22);
			this._imageBoxColumns.TabIndex = 4;
			this._imageBoxColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _imageBoxRows
			// 
			this._imageBoxRows.Font = new System.Drawing.Font("Arial", 9.75F);
			this._imageBoxRows.Location = new System.Drawing.Point(30, 58);
			this._imageBoxRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._imageBoxRows.Name = "_imageBoxRows";
			this._imageBoxRows.Size = new System.Drawing.Size(48, 22);
			this._imageBoxRows.TabIndex = 3;
			this._imageBoxRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// tilePanel
			// 
			this.tilePanel.Controls.Add(this.groupBox2);
			this.tilePanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.tilePanel.Location = new System.Drawing.Point(0, 155);
			this.tilePanel.Name = "tilePanel";
			this.tilePanel.Size = new System.Drawing.Size(225, 155);
			this.tilePanel.TabIndex = 20;
			// 
			// _tileColumns
			// 
			this._tileColumns.Font = new System.Drawing.Font("Arial", 9.75F);
			this._tileColumns.Location = new System.Drawing.Point(115, 54);
			this._tileColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._tileColumns.Name = "_tileColumns";
			this._tileColumns.Size = new System.Drawing.Size(48, 22);
			this._tileColumns.TabIndex = 9;
			this._tileColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _tileRows
			// 
			this._tileRows.Font = new System.Drawing.Font("Arial", 9.75F);
			this._tileRows.Location = new System.Drawing.Point(30, 54);
			this._tileRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._tileRows.Name = "_tileRows";
			this._tileRows.Size = new System.Drawing.Size(48, 22);
			this._tileRows.TabIndex = 8;
			this._tileRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this._applyTiles);
			this.groupBox2.Controls.Add(this._tileRowsLabel);
			this.groupBox2.Controls.Add(this._tileColumnsLabel);
			this.groupBox2.Controls.Add(this._tileRows);
			this.groupBox2.Controls.Add(this._tileColumns);
			this.groupBox2.Location = new System.Drawing.Point(15, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(193, 146);
			this.groupBox2.TabIndex = 11;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Tile";
			// 
			// LayoutControl
			// 
			this.Controls.Add(this.tilePanel);
			this.Controls.Add(this.imageBoxPanel);
			this.Name = "LayoutControl";
			this.Size = new System.Drawing.Size(225, 315);
			this.imageBoxPanel.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).EndInit();
			this.tilePanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._tileColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._tileRows)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

	}
}
