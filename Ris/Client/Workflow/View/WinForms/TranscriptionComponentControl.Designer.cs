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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class TranscriptionComponentControl
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
			this._reportEditorSplitContainer = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._transcriptiontEditorPanel = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._completeButton = new System.Windows.Forms.Button();
			this._rejectButton = new System.Windows.Forms.Button();
			this._saveButton = new System.Windows.Forms.Button();
			this._btnSkip = new System.Windows.Forms.Button();
			this._reportNextItem = new System.Windows.Forms.CheckBox();
			this._rightHandPanel = new System.Windows.Forms.Panel();
			this._statusText = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._bannerPanel = new System.Windows.Forms.Panel();
			this._reportEditorSplitContainer.Panel1.SuspendLayout();
			this._reportEditorSplitContainer.Panel2.SuspendLayout();
			this._reportEditorSplitContainer.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _reportEditorSplitContainer
			// 
			this._reportEditorSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reportEditorSplitContainer.Location = new System.Drawing.Point(3, 113);
			this._reportEditorSplitContainer.Name = "_reportEditorSplitContainer";
			// 
			// _reportEditorSplitContainer.Panel1
			// 
			this._reportEditorSplitContainer.Panel1.Controls.Add(this.tableLayoutPanel2);
			// 
			// _reportEditorSplitContainer.Panel2
			// 
			this._reportEditorSplitContainer.Panel2.Controls.Add(this._rightHandPanel);
			this._reportEditorSplitContainer.Size = new System.Drawing.Size(977, 801);
			this._reportEditorSplitContainer.SplitterDistance = 461;
			this._reportEditorSplitContainer.TabIndex = 0;
			this._reportEditorSplitContainer.TabStop = false;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this._cancelButton, 1, 3);
			this.tableLayoutPanel2.Controls.Add(this._transcriptiontEditorPanel, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 3);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 4;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(461, 801);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(383, 752);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 3;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _transcriptiontEditorPanel
			// 
			this._transcriptiontEditorPanel.AutoSize = true;
			this._transcriptiontEditorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.SetColumnSpan(this._transcriptiontEditorPanel, 2);
			this._transcriptiontEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._transcriptiontEditorPanel.Location = new System.Drawing.Point(3, 3);
			this._transcriptiontEditorPanel.Name = "_transcriptiontEditorPanel";
			this._transcriptiontEditorPanel.Size = new System.Drawing.Size(455, 743);
			this._transcriptiontEditorPanel.TabIndex = 0;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._completeButton);
			this.flowLayoutPanel1.Controls.Add(this._rejectButton);
			this.flowLayoutPanel1.Controls.Add(this._saveButton);
			this.flowLayoutPanel1.Controls.Add(this._btnSkip);
			this.flowLayoutPanel1.Controls.Add(this._reportNextItem);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 749);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(380, 52);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// _completeButton
			// 
			this._completeButton.Location = new System.Drawing.Point(3, 3);
			this._completeButton.Name = "_completeButton";
			this._completeButton.Size = new System.Drawing.Size(75, 23);
			this._completeButton.TabIndex = 0;
			this._completeButton.Text = "Complete";
			this._completeButton.UseVisualStyleBackColor = true;
			this._completeButton.Click += new System.EventHandler(this._completeButton_Click);
			// 
			// _rejectButton
			// 
			this._rejectButton.Location = new System.Drawing.Point(84, 3);
			this._rejectButton.Name = "_rejectButton";
			this._rejectButton.Size = new System.Drawing.Size(75, 23);
			this._rejectButton.TabIndex = 2;
			this._rejectButton.Text = "Reject";
			this._rejectButton.UseVisualStyleBackColor = true;
			this._rejectButton.Click += new System.EventHandler(this._rejectButton_Click);
			// 
			// _saveButton
			// 
			this._saveButton.Location = new System.Drawing.Point(165, 3);
			this._saveButton.Name = "_saveButton";
			this._saveButton.Size = new System.Drawing.Size(75, 23);
			this._saveButton.TabIndex = 3;
			this._saveButton.Text = "Save";
			this._saveButton.UseVisualStyleBackColor = true;
			this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
			// 
			// _btnSkip
			// 
			this._btnSkip.Location = new System.Drawing.Point(246, 3);
			this._btnSkip.Name = "_btnSkip";
			this._btnSkip.Size = new System.Drawing.Size(75, 23);
			this._btnSkip.TabIndex = 7;
			this._btnSkip.Text = "Skip";
			this._btnSkip.UseVisualStyleBackColor = true;
			this._btnSkip.Click += new System.EventHandler(this._btnSkip_Click);
			// 
			// _reportNextItem
			// 
			this._reportNextItem.AutoSize = true;
			this._reportNextItem.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reportNextItem.Location = new System.Drawing.Point(3, 32);
			this._reportNextItem.Name = "_reportNextItem";
			this._reportNextItem.Size = new System.Drawing.Size(112, 17);
			this._reportNextItem.TabIndex = 8;
			this._reportNextItem.Text = "Go To Next Item";
			this._reportNextItem.UseVisualStyleBackColor = true;
			// 
			// _rightHandPanel
			// 
			this._rightHandPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._rightHandPanel.Location = new System.Drawing.Point(0, 0);
			this._rightHandPanel.Name = "_rightHandPanel";
			this._rightHandPanel.Size = new System.Drawing.Size(512, 801);
			this._rightHandPanel.TabIndex = 0;
			// 
			// _statusText
			// 
			this._statusText.AutoSize = true;
			this._statusText.BackColor = System.Drawing.Color.LightSteelBlue;
			this._statusText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._statusText.Dock = System.Windows.Forms.DockStyle.Fill;
			this._statusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._statusText.ForeColor = System.Drawing.SystemColors.ControlText;
			this._statusText.Location = new System.Drawing.Point(3, 88);
			this._statusText.Margin = new System.Windows.Forms.Padding(3);
			this._statusText.Name = "_statusText";
			this._statusText.Padding = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this._statusText.Size = new System.Drawing.Size(977, 19);
			this._statusText.TabIndex = 2;
			this._statusText.Text = "Transcribing from X worklist - Y items available - Z items completed";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._statusText, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._reportEditorSplitContainer, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._bannerPanel, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(983, 917);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// _bannerPanel
			// 
			this._bannerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bannerPanel.Location = new System.Drawing.Point(3, 3);
			this._bannerPanel.Name = "_bannerPanel";
			this._bannerPanel.Size = new System.Drawing.Size(977, 79);
			this._bannerPanel.TabIndex = 0;
			// 
			// TranscriptionComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "TranscriptionComponentControl";
			this.Size = new System.Drawing.Size(983, 917);
			this._reportEditorSplitContainer.Panel1.ResumeLayout(false);
			this._reportEditorSplitContainer.Panel1.PerformLayout();
			this._reportEditorSplitContainer.Panel2.ResumeLayout(false);
			this._reportEditorSplitContainer.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.SplitContainer _reportEditorSplitContainer;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Panel _transcriptiontEditorPanel;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _completeButton;
		private System.Windows.Forms.Button _rejectButton;
		private System.Windows.Forms.Button _saveButton;
		private System.Windows.Forms.Button _btnSkip;
		private System.Windows.Forms.CheckBox _reportNextItem;
		private System.Windows.Forms.Panel _rightHandPanel;
		private System.Windows.Forms.Label _statusText;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel _bannerPanel;
    }
}