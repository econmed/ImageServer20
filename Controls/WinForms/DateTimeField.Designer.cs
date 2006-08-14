namespace ClearCanvas.Controls.WinForms
{
    partial class DateTimeField
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer _components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
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
            this._dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this._label = new System.Windows.Forms.Label();
            this._checkBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _dateTimePicker
            // 
            this._dateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dateTimePicker.Location = new System.Drawing.Point(2, 20);
            this._dateTimePicker.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._dateTimePicker.Name = "_dateTimePicker";
            this._dateTimePicker.Size = new System.Drawing.Size(146, 20);
            this._dateTimePicker.TabIndex = 0;
            // 
            // _label
            // 
            this._label.AutoSize = true;
            this._label.Location = new System.Drawing.Point(2, 3);
            this._label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(29, 13);
            this._label.TabIndex = 1;
            this._label.Text = "label";
            // 
            // _checkBox
            // 
            this._checkBox.AutoSize = true;
            this._checkBox.Location = new System.Drawing.Point(4, 1);
            this._checkBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._checkBox.Name = "_checkBox";
            this._checkBox.Size = new System.Drawing.Size(74, 17);
            this._checkBox.TabIndex = 2;
            this._checkBox.Text = "checkBox";
            this._checkBox.UseVisualStyleBackColor = true;
            // 
            // DateTimeField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._label);
            this.Controls.Add(this._dateTimePicker);
            this.Controls.Add(this._checkBox);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "DateTimeField";
            this.Size = new System.Drawing.Size(150, 41);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker _dateTimePicker;
        private System.Windows.Forms.Label _label;
        private System.Windows.Forms.CheckBox _checkBox;
    }
}
