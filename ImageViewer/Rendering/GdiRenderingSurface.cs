#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Rendering
{
	internal sealed class GdiRenderingSurface : IRenderingSurface
	{
		private ImageBuffer _imageBuffer;
		private ImageBuffer _finalBuffer;
		private IntPtr _windowID;
		private IntPtr _contextID;
		private Rectangle _clientRectangle;
		private Rectangle _clipRectangle;

		public GdiRenderingSurface(IntPtr windowID, int width, int height)
		{
			if (width == 0 || height == 0)
				return;

			this.ClientRectangle = new Rectangle(0, 0, width, height);

			_windowID = windowID;
		}


		#region IRenderingSurface Members

		public IntPtr WindowID
		{
			get { return _windowID; }
			set { _windowID = value; }
		}

		public IntPtr ContextID 
		{
			get { return _contextID; }
			set { _contextID = value; }
		}

		/// <summary>
		/// Gets or sets the rectangle to which the image will be rendered.
		/// </summary>
		/// <remarks>
		/// This is typically the rectangle of the view onto the <see cref="ITile"/>.
		/// </remarks>
		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
			set
			{
				if (_clientRectangle != value)
				{
					_clientRectangle = value;
					CreateBuffers(_clientRectangle.Width, _clientRectangle.Height);
				}
			}
		}

		/// <summary>
		/// Gets or sets the rectangle that requires repainting.
		/// </summary>
		/// <remarks>
		/// The implementer of <see cref="IRenderer"/> should use this rectangle
		/// to intelligently perform the <see cref="DrawMode.Refresh"/> operation.
		/// </remarks>
		public Rectangle ClipRectangle
		{
			get { return _clipRectangle; }
			set { _clipRectangle = value; }
		}


		#endregion

		public ImageBuffer ImageBuffer
		{
			get { return _imageBuffer; }
		}

		public ImageBuffer FinalBuffer
		{
			get { return _finalBuffer; }
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeOffscreenBuffers();
			}
		}

		private void DisposeOffscreenBuffers()
		{
			if (_imageBuffer != null)
			{
				_imageBuffer.Dispose();
				_imageBuffer = null;
			}
			if (_finalBuffer != null)
			{
				_finalBuffer.Dispose();
				_finalBuffer = null;
			}
		}

		private void CreateBuffers(int width, int height)
		{
			DisposeOffscreenBuffers();

			_imageBuffer = new ImageBuffer(width, height);
			_finalBuffer = new ImageBuffer(width, height);
		}
	}
}
