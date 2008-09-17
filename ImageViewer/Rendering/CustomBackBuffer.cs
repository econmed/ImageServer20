using System;
using System.Drawing;
using System.Drawing.Imaging;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Rendering
{
	internal class CustomBackBuffer : BackBuffer
	{
		private ImageBuffer _buffer;
		private ImageBuffer _indexedBuffer;

		public CustomBackBuffer(bool useIndexedBuffer)
		{
			_buffer = new ImageBuffer(false);
			_indexedBuffer = new ImageBuffer(useIndexedBuffer);
		}
		
		public override System.Drawing.Graphics Graphics
		{
			get { return _buffer.Graphics; }	
		}

		protected override ImageBuffer ColorBuffer
		{
			get { return _buffer; }
		}

		public override void RenderToScreen()
		{
			if (IsClientRectangleEmpty)
				return;

			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromHdc(ContextID))
			{
				if (_indexedBuffer != null)
				{
					_indexedBuffer.Render(_buffer);
					g.DrawImageUnscaled(_indexedBuffer.Bitmap, 0, 0);
				}
				else
				{
					g.DrawImageUnscaled(_buffer.Bitmap, 0, 0);
				}
			}
		}

		protected override void OnClientRectangleChanged()
		{
			if (_buffer != null)
				_buffer.Size = new Size(ClientRectangle.Width, ClientRectangle.Height);

			if (_indexedBuffer != null)
				_indexedBuffer.Size = new Size(ClientRectangle.Width, ClientRectangle.Height);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_buffer != null)
				{
					_buffer.Dispose();
					_buffer = null;
				}
				if (_indexedBuffer != null)
				{
					_indexedBuffer.Dispose();
					_indexedBuffer = null;
				}
			}
		}
	}
}