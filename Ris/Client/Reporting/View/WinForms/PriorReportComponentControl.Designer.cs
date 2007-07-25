namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    partial class PriorReportComponentControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._reportList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._browser = new System.Windows.Forms.WebBrowser();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._reportList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._browser);
            this.splitContainer1.Size = new System.Drawing.Size(443, 428);
            this.splitContainer1.SplitterDistance = 181;
            this.splitContainer1.TabIndex = 1;
            // 
            // _reportList
            // 
            this._reportList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._reportList.Location = new System.Drawing.Point(0, 0);
            this._reportList.MenuModel = null;
            this._reportList.Name = "_reportList";
            this._reportList.ReadOnly = false;
            this._reportList.Selection = selection1;
            this._reportList.Size = new System.Drawing.Size(443, 181);
            this._reportList.TabIndex = 2;
            this._reportList.Table = null;
            this._reportList.ToolbarModel = null;
            this._reportList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._reportList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _browser
            // 
            this._browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._browser.Location = new System.Drawing.Point(0, 0);
            this._browser.MinimumSize = new System.Drawing.Size(20, 20);
            this._browser.Name = "_browser";
            this._browser.Size = new System.Drawing.Size(443, 243);
            this._browser.TabIndex = 0;
            // 
            // PriorReportComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PriorReportComponentControl";
            this.Size = new System.Drawing.Size(443, 428);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ClearCanvas.Desktop.View.WinForms.TableView _reportList;
        private System.Windows.Forms.WebBrowser _browser;

    }
}
