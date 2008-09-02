namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class LinkedInterpretationComponentControl
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
			this._worklistItemTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._okButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._sourceWorklistItem = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.SuspendLayout();
			// 
			// _worklistItemTableView
			// 
			this._worklistItemTableView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._worklistItemTableView.Location = new System.Drawing.Point(13, 133);
			this._worklistItemTableView.Name = "_worklistItemTableView";
			this._worklistItemTableView.ReadOnly = false;
			this._worklistItemTableView.ShowToolbar = false;
			this._worklistItemTableView.Size = new System.Drawing.Size(725, 315);
			this._worklistItemTableView.TabIndex = 1;
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.Location = new System.Drawing.Point(662, 465);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 2;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 117);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(324, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Check any additional procedures that should be linked to this report";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(10, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Reporting:";
			// 
			// _selectedItem
			// 
			this._sourceWorklistItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._sourceWorklistItem.Location = new System.Drawing.Point(13, 26);
			this._sourceWorklistItem.Name = "_sourceWorklistItem";
			this._sourceWorklistItem.ReadOnly = false;
			this._sourceWorklistItem.ShowToolbar = false;
			this._sourceWorklistItem.Size = new System.Drawing.Size(725, 71);
			this._sourceWorklistItem.TabIndex = 4;
			// 
			// LinkedInterpretationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._sourceWorklistItem);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._worklistItemTableView);
			this.Name = "LinkedInterpretationComponentControl";
			this.Size = new System.Drawing.Size(754, 501);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _worklistItemTableView;
		private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private ClearCanvas.Desktop.View.WinForms.TableView _sourceWorklistItem;
    }
}
