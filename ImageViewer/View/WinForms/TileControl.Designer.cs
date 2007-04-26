using System;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    partial class TileControl
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
            if (disposing)
            {
				if (components != null)
					components.Dispose();

				if (_tile != null)
				{
					_tile.Drawing -= new EventHandler(OnDrawing);
					_tile.RendererChanged -= new EventHandler(OnRendererChanged);
					_tile.InformationBoxChanged -= new EventHandler<InformationBoxChangedEventArgs>(OnInformationBoxChanged);
					_tileController.CursorTokenChanged -= new EventHandler(OnCursorTokenChanged);
					_tileController.CaptureChanging -= new EventHandler<CaptureChangingEventArgs>(OnCaptureChanging);

					_tile = null;
				}
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
			this.components = new System.ComponentModel.Container();
			this._contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// _contextMenuStrip
			// 
			this._contextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._contextMenuStrip.Name = "_contextMenuStrip";
			this._contextMenuStrip.Size = new System.Drawing.Size(153, 26);
			// 
			// _toolTip
			// 
			this._toolTip.Active = false;
			this._toolTip.AutomaticDelay = 0;
			this._toolTip.UseAnimation = false;
			this._toolTip.UseFading = false;
			// 
			// TileControl
			// 
			this.ContextMenuStrip = this._contextMenuStrip;
			this.Name = "TileControl";
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.ContextMenuStrip _contextMenuStrip;
		private System.Windows.Forms.ToolTip _toolTip;
    }
}
