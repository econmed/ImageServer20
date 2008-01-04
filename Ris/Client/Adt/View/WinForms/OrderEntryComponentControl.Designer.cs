#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class OrderEntryComponentControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderEntryComponentControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._orderingFacility = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._visitSummaryButton = new System.Windows.Forms.Button();
            this._schedulingRequestTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._schedulingRequestDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._visit = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._orderingPractitioner = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
            this._indication = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._reorderReason = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._priority = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._diagnosticService = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this._proceduresTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._orderNotesTab = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._consultantsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.panel2 = new System.Windows.Forms.Panel();
            this._consultantLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
            this._addConsultantButton = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this._documentsTab = new System.Windows.Forms.TabPage();
            this._cancelButton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this._bannerPanel = new System.Windows.Forms.Panel();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(2, 87);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._orderingFacility);
            this.splitContainer1.Panel1.Controls.Add(this._visitSummaryButton);
            this.splitContainer1.Panel1.Controls.Add(this._schedulingRequestTime);
            this.splitContainer1.Panel1.Controls.Add(this._schedulingRequestDate);
            this.splitContainer1.Panel1.Controls.Add(this._visit);
            this.splitContainer1.Panel1.Controls.Add(this._orderingPractitioner);
            this.splitContainer1.Panel1.Controls.Add(this._indication);
            this.splitContainer1.Panel1.Controls.Add(this._reorderReason);
            this.splitContainer1.Panel1.Controls.Add(this._priority);
            this.splitContainer1.Panel1.Controls.Add(this._diagnosticService);
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl2);
            this.splitContainer1.Size = new System.Drawing.Size(987, 525);
            this.splitContainer1.SplitterDistance = 496;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 26;
            // 
            // _orderingFacility
            // 
            this._orderingFacility.LabelText = "Ordering Facility";
            this._orderingFacility.Location = new System.Drawing.Point(5, 1);
            this._orderingFacility.Margin = new System.Windows.Forms.Padding(2);
            this._orderingFacility.Mask = "";
            this._orderingFacility.Name = "_orderingFacility";
            this._orderingFacility.ReadOnly = true;
            this._orderingFacility.Size = new System.Drawing.Size(150, 41);
            this._orderingFacility.TabIndex = 12;
            this._orderingFacility.ToolTip = null;
            this._orderingFacility.Value = null;
            // 
            // _visitSummaryButton
            // 
            this._visitSummaryButton.Image = ((System.Drawing.Image)(resources.GetObject("_visitSummaryButton.Image")));
            this._visitSummaryButton.Location = new System.Drawing.Point(463, 209);
            this._visitSummaryButton.Name = "_visitSummaryButton";
            this._visitSummaryButton.Size = new System.Drawing.Size(24, 24);
            this._visitSummaryButton.TabIndex = 11;
            this._visitSummaryButton.UseVisualStyleBackColor = true;
            this._visitSummaryButton.Click += new System.EventHandler(this._visitSummaryButton_Click);
            // 
            // _schedulingRequestTime
            // 
            this._schedulingRequestTime.LabelText = "Requested Schedule Time";
            this._schedulingRequestTime.Location = new System.Drawing.Point(175, 237);
            this._schedulingRequestTime.Margin = new System.Windows.Forms.Padding(2);
            this._schedulingRequestTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._schedulingRequestTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._schedulingRequestTime.Name = "_schedulingRequestTime";
            this._schedulingRequestTime.Nullable = true;
            this._schedulingRequestTime.ShowDate = false;
            this._schedulingRequestTime.ShowTime = true;
            this._schedulingRequestTime.Size = new System.Drawing.Size(137, 41);
            this._schedulingRequestTime.TabIndex = 8;
            this._schedulingRequestTime.Value = null;
            // 
            // _schedulingRequestDate
            // 
            this._schedulingRequestDate.LabelText = "Requested Schedule Date";
            this._schedulingRequestDate.Location = new System.Drawing.Point(5, 237);
            this._schedulingRequestDate.Margin = new System.Windows.Forms.Padding(2);
            this._schedulingRequestDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._schedulingRequestDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._schedulingRequestDate.Name = "_schedulingRequestDate";
            this._schedulingRequestDate.Nullable = true;
            this._schedulingRequestDate.Size = new System.Drawing.Size(166, 41);
            this._schedulingRequestDate.TabIndex = 7;
            this._schedulingRequestDate.Value = null;
            // 
            // _visit
            // 
            this._visit.DataSource = null;
            this._visit.DisplayMember = "";
            this._visit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._visit.LabelText = "Visit";
            this._visit.Location = new System.Drawing.Point(2, 192);
            this._visit.Margin = new System.Windows.Forms.Padding(2);
            this._visit.Name = "_visit";
            this._visit.Size = new System.Drawing.Size(460, 41);
            this._visit.TabIndex = 6;
            this._visit.Value = null;
            // 
            // _orderingPractitioner
            // 
            this._orderingPractitioner.LabelText = "Ordering Practitioner";
            this._orderingPractitioner.Location = new System.Drawing.Point(218, 98);
            this._orderingPractitioner.Margin = new System.Windows.Forms.Padding(2);
            this._orderingPractitioner.Name = "_orderingPractitioner";
            this._orderingPractitioner.Size = new System.Drawing.Size(269, 48);
            this._orderingPractitioner.TabIndex = 4;
            this._orderingPractitioner.Value = null;
            // 
            // _indication
            // 
            this._indication.LabelText = "Indication";
            this._indication.Location = new System.Drawing.Point(2, 147);
            this._indication.Margin = new System.Windows.Forms.Padding(2);
            this._indication.Mask = "";
            this._indication.Name = "_indication";
            this._indication.Size = new System.Drawing.Size(485, 41);
            this._indication.TabIndex = 5;
            this._indication.ToolTip = null;
            this._indication.Value = null;
            // 
            // _reorderReason
            // 
            this._reorderReason.DataSource = null;
            this._reorderReason.DisplayMember = "";
            this._reorderReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._reorderReason.LabelText = "Re-order Reason";
            this._reorderReason.Location = new System.Drawing.Point(180, 1);
            this._reorderReason.Margin = new System.Windows.Forms.Padding(2);
            this._reorderReason.Name = "_reorderReason";
            this._reorderReason.Size = new System.Drawing.Size(282, 41);
            this._reorderReason.TabIndex = 1;
            this._reorderReason.Value = null;
            // 
            // _priority
            // 
            this._priority.DataSource = null;
            this._priority.DisplayMember = "";
            this._priority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._priority.LabelText = "Priority";
            this._priority.Location = new System.Drawing.Point(2, 102);
            this._priority.Margin = new System.Windows.Forms.Padding(2);
            this._priority.Name = "_priority";
            this._priority.Size = new System.Drawing.Size(201, 41);
            this._priority.TabIndex = 3;
            this._priority.Value = null;
            // 
            // _diagnosticService
            // 
            this._diagnosticService.LabelText = "Diagnostic Service";
            this._diagnosticService.Location = new System.Drawing.Point(2, 46);
            this._diagnosticService.Margin = new System.Windows.Forms.Padding(2);
            this._diagnosticService.Name = "_diagnosticService";
            this._diagnosticService.Size = new System.Drawing.Size(485, 48);
            this._diagnosticService.TabIndex = 2;
            this._diagnosticService.Value = null;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this._orderNotesTab);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 289);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(492, 217);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this._proceduresTableView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(484, 191);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Procedures";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // _proceduresTableView
            // 
            this._proceduresTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._proceduresTableView.FilterTextBoxWidth = 132;
            this._proceduresTableView.Location = new System.Drawing.Point(2, 2);
            this._proceduresTableView.Margin = new System.Windows.Forms.Padding(4);
            this._proceduresTableView.MultiSelect = false;
            this._proceduresTableView.Name = "_proceduresTableView";
            this._proceduresTableView.ReadOnly = false;
            this._proceduresTableView.ShowToolbar = false;
            this._proceduresTableView.Size = new System.Drawing.Size(480, 187);
            this._proceduresTableView.TabIndex = 0;
            this._proceduresTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._proceduresTableView.ItemDoubleClicked += new System.EventHandler(this._proceduresTableView_ItemDoubleClicked);
            // 
            // _orderNotesTab
            // 
            this._orderNotesTab.Location = new System.Drawing.Point(4, 22);
            this._orderNotesTab.Margin = new System.Windows.Forms.Padding(2);
            this._orderNotesTab.Name = "_orderNotesTab";
            this._orderNotesTab.Size = new System.Drawing.Size(484, 191);
            this._orderNotesTab.TabIndex = 2;
            this._orderNotesTab.Text = "Notes";
            this._orderNotesTab.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(484, 191);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Copies To";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._consultantsTableView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(480, 187);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _consultantsTableView
            // 
            this._consultantsTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._consultantsTableView.FilterTextBoxWidth = 132;
            this._consultantsTableView.Location = new System.Drawing.Point(4, 66);
            this._consultantsTableView.Margin = new System.Windows.Forms.Padding(4);
            this._consultantsTableView.MultiSelect = false;
            this._consultantsTableView.Name = "_consultantsTableView";
            this._consultantsTableView.ReadOnly = false;
            this._consultantsTableView.ShowToolbar = false;
            this._consultantsTableView.Size = new System.Drawing.Size(472, 117);
            this._consultantsTableView.TabIndex = 2;
            this._consultantsTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this._consultantLookup);
            this.panel2.Controls.Add(this._addConsultantButton);
            this.panel2.Location = new System.Drawing.Point(2, 2);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(362, 58);
            this.panel2.TabIndex = 0;
            // 
            // _consultantLookup
            // 
            this._consultantLookup.LabelText = "Find Practitioner";
            this._consultantLookup.Location = new System.Drawing.Point(1, 5);
            this._consultantLookup.Margin = new System.Windows.Forms.Padding(2);
            this._consultantLookup.Name = "_consultantLookup";
            this._consultantLookup.Size = new System.Drawing.Size(261, 48);
            this._consultantLookup.TabIndex = 4;
            this._consultantLookup.Value = null;
            // 
            // _addConsultantButton
            // 
            this._addConsultantButton.Location = new System.Drawing.Point(276, 27);
            this._addConsultantButton.Margin = new System.Windows.Forms.Padding(2);
            this._addConsultantButton.Name = "_addConsultantButton";
            this._addConsultantButton.Size = new System.Drawing.Size(75, 23);
            this._addConsultantButton.TabIndex = 5;
            this._addConsultantButton.Text = "Add";
            this._addConsultantButton.UseVisualStyleBackColor = true;
            this._addConsultantButton.Click += new System.EventHandler(this._addConsultantButton_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this._documentsTab);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(488, 525);
            this.tabControl2.TabIndex = 0;
            // 
            // _documentsTab
            // 
            this._documentsTab.Location = new System.Drawing.Point(4, 22);
            this._documentsTab.Margin = new System.Windows.Forms.Padding(2);
            this._documentsTab.Name = "_documentsTab";
            this._documentsTab.Padding = new System.Windows.Forms.Padding(2);
            this._documentsTab.Size = new System.Drawing.Size(480, 499);
            this._documentsTab.TabIndex = 0;
            this._documentsTab.Text = "Documents";
            this._documentsTab.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(919, 615);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 29;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(840, 615);
            this._acceptButton.Margin = new System.Windows.Forms.Padding(2);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 28;
            this._acceptButton.Text = "OK";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._placeOrderButton_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this._bannerPanel, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(991, 614);
            this.tableLayoutPanel2.TabIndex = 30;
            // 
            // _bannerPanel
            // 
            this._bannerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._bannerPanel.Location = new System.Drawing.Point(3, 3);
            this._bannerPanel.Name = "_bannerPanel";
            this._bannerPanel.Size = new System.Drawing.Size(985, 79);
            this._bannerPanel.TabIndex = 27;
            // 
            // OrderEntryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._acceptButton);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "OrderEntryComponentControl";
            this.Size = new System.Drawing.Size(1000, 640);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ClearCanvas.Desktop.View.WinForms.TableView _proceduresTableView;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage _orderNotesTab;
        private System.Windows.Forms.TabPage tabPage2;
        private ClearCanvas.Desktop.View.WinForms.TableView _consultantsTableView;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage _documentsTab;
        private ClearCanvas.Ris.Client.View.WinForms.LookupField _diagnosticService;
        private System.Windows.Forms.Button _addConsultantButton;
        private ClearCanvas.Ris.Client.View.WinForms.LookupField _consultantLookup;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _visit;
        private ClearCanvas.Ris.Client.View.WinForms.LookupField _orderingPractitioner;
        private ClearCanvas.Desktop.View.WinForms.TextField _indication;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _reorderReason;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _priority;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _schedulingRequestTime;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _schedulingRequestDate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button _visitSummaryButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _orderingFacility;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel _bannerPanel;

    }
}
