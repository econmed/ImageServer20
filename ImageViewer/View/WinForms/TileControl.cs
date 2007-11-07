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

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TileComponent"/>
    /// </summary>
    public partial class TileControl : UserControl
	{
		#region Private fields

		private Tile _tile;
		private TileInputTranslator _inputTranslator;
		private TileController _tileController;

		private InformationBox _currentInformationBox;

		private IRenderingSurface _surface;
		private IMouseButtonHandler _currentMouseButtonHandler;
		private CursorWrapper _currentCursorWrapper;

		#endregion

		/// <summary>
        /// Constructor
        /// </summary>
        public TileControl(Tile tile, Rectangle parentRectangle, int parentImageBoxInsetWidth)
        {
			_tile = tile;
			_tileController = new TileController(_tile, (_tile.ImageViewer as ImageViewerComponent).ShortcutManager);
			_inputTranslator = new TileInputTranslator(this);

			SetParentImageBoxRectangle(parentRectangle, parentImageBoxInsetWidth);
			InitializeComponent();

			this.BackColor = Color.Black;
			this.Dock = DockStyle.None;
			this.Anchor = AnchorStyles.None;

			_tile.Drawing += new EventHandler(OnDrawing);
			_tile.RendererChanged += new EventHandler(OnRendererChanged);
			_tile.InformationBoxChanged += new EventHandler<InformationBoxChangedEventArgs>(OnInformationBoxChanged);

			_contextMenuStrip.ImageScalingSize = new Size(24, 24);
			_contextMenuStrip.Opening += new CancelEventHandler(OnContextMenuStripOpening);

			_tileController.CursorTokenChanged += new EventHandler(OnCursorTokenChanged);
			_tileController.CaptureChanging += new EventHandler<ItemEventArgs<IMouseButtonHandler>>(OnCaptureChanging);
		}

		public Tile Tile
		{
			get { return _tile; }
		}

		private IRenderingSurface Surface
		{
			get 
			{
				if (_surface == null)
				{
					// TileControl should *always* have an associated Tile
					if (this.Tile == null)
						throw new Exception(SR.ExceptionTileControlNoAssociatedTile);

					// Legitimate case; Tile maybe empty
					if (this.Tile.PresentationImage == null)
						return null;

					IRenderer renderer = ((PresentationImage)Tile.PresentationImage).ImageRenderer;

					// PresntationImage should *always* have a renderer
					if (renderer == null)
						throw new Exception(SR.ExceptionPresentationImageNotAssociatedWithARenderer);

					_surface = renderer.GetRenderingSurface(this.Handle, this.Width, this.Height);
				}

				return _surface; 
			}
		}

		public void SetParentImageBoxRectangle(
			Rectangle parentImageBoxRectangle, 
			int parentImageBoxBorderWidth)
		{
			int insetImageBoxWidth = parentImageBoxRectangle.Width - 2 * parentImageBoxBorderWidth;
			int insetImageBoxHeight = parentImageBoxRectangle.Height - 2 * parentImageBoxBorderWidth;

			int left = (int)(_tile.NormalizedRectangle.Left * insetImageBoxWidth + Tile.InsetWidth);
			int top = (int)(_tile.NormalizedRectangle.Top * insetImageBoxHeight + Tile.InsetWidth);
			int right = (int)(_tile.NormalizedRectangle.Right * insetImageBoxWidth - Tile.InsetWidth);
			int bottom = (int)(_tile.NormalizedRectangle.Bottom * insetImageBoxHeight - Tile.InsetWidth);

			this.SuspendLayout();

			this.Location = new Point(left + parentImageBoxBorderWidth, top + parentImageBoxBorderWidth);
			this.Size = new Size(right - left, bottom - top);
			this.ResumeLayout(false);
		}
		
		public void Draw()
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			if (this.Surface != null)
			{
				this.Surface.ClientRectangle = this.ClientRectangle;
				this.Surface.ClipRectangle = this.ClientRectangle;
			}

			DrawArgs args = new DrawArgs(
				this.Surface, 
				ClearCanvas.ImageViewer.Rendering.DrawMode.Render);
			_tile.OnDraw(args);
			Invalidate();
			Update();

			clock.Stop();
			string str = String.Format("TileControl.Draw: {0}, {1}\n", clock.ToString(), this.Size.ToString());
			Trace.Write(str);
		}

		private void DisposeSurface()
		{
			try
			{
				if (_surface != null)
					_surface.Dispose();
			}
			finally
			{
				_surface = null;
			}
		}

    	private void OnDrawing(object sender, EventArgs e)
		{
			Draw();
		}

		private void OnRendererChanged(object sender, EventArgs e)
		{
			DisposeSurface();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			// Make sure tile gets blacked out if there's
			// no presentation image in it
			if (_tile.PresentationImage == null)
				DisposeSurface();

			if (this.Surface == null)
			{
				e.Graphics.Clear(Color.Black);
			}
			else
			{
				this.Surface.ContextID = e.Graphics.GetHdc();
				this.Surface.ClientRectangle = this.ClientRectangle;
				this.Surface.ClipRectangle = e.ClipRectangle;

				DrawArgs args = new DrawArgs(
					this.Surface, 
					ClearCanvas.ImageViewer.Rendering.DrawMode.Refresh);
				
				_tile.OnDraw(args);
				
				e.Graphics.ReleaseHdc(this.Surface.ContextID);
			}

			base.OnPaint(e);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// We're double buffering manually, so override this and do nothing
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);

		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			Trace.Write("TileControl.OnSizeChanged()\n");

			_tileController.TileClientRectangle = this.ClientRectangle;

			// Set the surface to null so when it's accessed, a new surface
			// will be created.
			DisposeSurface();
			Draw();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			object message = _inputTranslator.OnMouseLeave();
			if (message == null)
				return;

			_tileController.ProcessMessage(message);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Focus();

			object message = _inputTranslator.OnMouseDown(e);
			if (message == null)
				return;

			_tileController.ProcessMessage(message);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			object message = _inputTranslator.OnMouseMove(e);
			if (message == null)
				return;

			_tileController.ProcessMessage(message);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			object message = _inputTranslator.OnMouseUp(e);
			if (message == null)
				return;

			_tileController.ProcessMessage(message);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			object message = _inputTranslator.OnMouseWheel(e);
			if (message == null)
				return;

			_tileController.ProcessMessage(message);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			object message = _inputTranslator.OnKeyDown(e);
			if (message == null)
				return;

			if (_tileController.ProcessMessage(message))
				e.Handled = true;

			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			object message = _inputTranslator.OnKeyUp(e);
			if (message == null)
				return;

			if (_tileController.ProcessMessage(message))
				e.Handled = true;

			base.OnKeyUp(e);
		}

		protected override bool IsInputKey(Keys keyData)
		{
			//We want the tile control to receive keydown messages for *all* keys.
			return true;
		}

		public override bool PreProcessMessage(ref Message msg)
		{
			object message = _inputTranslator.PreProcessMessage(msg);
			if (message != null)
				_tileController.ProcessMessage(message);

			bool returnValue = base.PreProcessMessage(ref msg);

			message = _inputTranslator.PostProcessMessage(msg, returnValue);
			if (message != null)
				_tileController.ProcessMessage(message);

			return returnValue;
		}

		protected override void OnMouseCaptureChanged(EventArgs e)
		{
			base.OnMouseCaptureChanged(e);

			//This feels bad to me, but it's the only way to accomplish
			//keeping the capture when the mouse has come up.  .NET automatically handles
			//capture for you by turning it on on mouse down and off on mouse up, but
			//it does not allow you to keep capture when the mouse is not down.  Even
			// if you take out the calls to the base class OnMouseX handlers, it still
			// turns Capture back off although it has been turned on explicitly.
			if (this._currentMouseButtonHandler != null)
				this.Capture = true;
			}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			// Notify the surface that the tile control's window handle is
			// about to be destroyed so that any objects using the handle have
			// a chance to deal with it
			if (_surface != null)
				_surface.WindowID = IntPtr.Zero;

			base.OnHandleDestroyed(e);
		}

		private void OnCaptureChanging(object sender, ItemEventArgs<IMouseButtonHandler> e)
		{
			if (_currentMouseButtonHandler == e.Item)
				return;

			_currentMouseButtonHandler = e.Item;
			this.Capture = (_currentMouseButtonHandler != null);
		}

		private void OnCursorTokenChanged(object sender, EventArgs e)
		{
			if (_tileController.CursorToken == null)
			{
				this.Cursor = this.DefaultCursor;

				if (_currentCursorWrapper != null)
				{
					_currentCursorWrapper.Dispose();
					_currentCursorWrapper = null;
				}
			}
			else
			{
				try
				{
					CursorWrapper oldCursorWrapper = _currentCursorWrapper;
					_currentCursorWrapper = CursorFactory.CreateCursor(_tileController.CursorToken);
					this.Cursor = _currentCursorWrapper.Cursor;

					if (oldCursorWrapper != null)
						oldCursorWrapper.Dispose();
				}
				catch (Exception exception)
				{
					Platform.Log(LogLevel.Error, exception);
					this.Cursor = this.DefaultCursor;
					_currentCursorWrapper = null;
				}
			}
		}

		private void OnContextMenuStripOpening(object sender, CancelEventArgs e)
		{
			if (_tileController.ContextMenuProvider == null)
			{
				e.Cancel = true;
				return;
			}

			if (_tileController.ContextMenuEnabled)
			{
				ActionModelNode menuModel = _tileController.ContextMenuProvider.GetContextMenuModel(_tileController); 
				ToolStripBuilder.Clear(_contextMenuStrip.Items);
				ToolStripBuilder.BuildMenu(_contextMenuStrip.Items, menuModel.ChildNodes);
				e.Cancel = false;
			}
			else
				e.Cancel = true;
		}

		private void OnInformationBoxChanged(object sender, InformationBoxChangedEventArgs e)
		{
			if (_currentInformationBox != null)
				_currentInformationBox.Updated -= new EventHandler(OnUpdateInformationBox);

			_currentInformationBox = e.InformationBox;
			
			_toolTip.Active = false;
			_toolTip.Hide(this);

			if (e.InformationBox != null)
				_currentInformationBox.Updated += new EventHandler(OnUpdateInformationBox);
		}

		private void OnUpdateInformationBox(object sender, EventArgs e)
		{
			if (!_currentInformationBox.Visible)
			{
				_toolTip.Active = false;
				_toolTip.Hide(this);
			}
			else
			{
				_toolTip.Active = true;
				Point point = new Point(_currentInformationBox.DestinationPoint.X, _currentInformationBox.DestinationPoint.Y);
				point.Offset(5, 5);
				_toolTip.Show(_currentInformationBox.Data, this, point);
			}
		}
    }
}
