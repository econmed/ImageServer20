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

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Imaging
{
	///<summary>
	/// Used by the <see cref="PixelData"/> class to (lazily) retrieve pixel data from an external source.
	///</summary>
	public delegate byte[] PixelDataGetter();

	/// <summary>
	/// A pixel data wrapper.
	/// </summary>
	/// <remarks>
	/// <see cref="PixelData"/> provides a number of convenience methods
	/// to make accessing and changing pixel data easier.  Use these methods
	/// judiciously, as the convenience comes at the expense of performance.
	/// For example, if you're doing complex image processing, using methods
	/// such as <see cref="SetPixel(int, int, int)"/> is not recommended if you want
	/// good performance.  Instead, use the <see cref="Raw"/> property 
	/// to get the raw byte array, then use unsafe code to do your processing.
	/// </remarks>
	/// <seealso cref="PixelDataGetter"/>
	public abstract class PixelData
	{
		#region Private fields

		/// <summary>
		/// The number of rows in the pixel data.
		/// </summary>
		protected int _rows;
		/// <summary>
		/// The number of columns in the pixel data.
		/// </summary>
		protected int _columns;
		/// <summary>
		/// The number of bits allocated the pixel data.
		/// </summary>
		protected int _bitsAllocated;
		/// <summary>
		/// The pixel data.
		/// </summary>
		protected byte[] _pixelData;
		/// <summary>
		/// A delegate used to retrieve the pixel data.
		/// </summary>
		protected PixelDataGetter _pixelDataGetter;
		/// <summary>
		/// The number of bytes per pixel the pixel data.
		/// </summary>
		protected int _bytesPerPixel;

		private int _stride;

		#endregion

		#region Protected constructor

		/// <summary>
		/// Initializes a new instance of <see cref="PixelData"/> with the specified image parameters.
		/// </summary>
		/// <param name="rows">The number of rows.</param>
		/// <param name="columns">The number of columns.</param>
		/// <param name="bitsAllocated">The number of bits allocated in the <paramref name="pixelData"/>.</param>
		/// <param name="pixelData">The pixel data to be wrapped.</param>
		protected PixelData(
			int rows,
			int columns,
			int bitsAllocated,
			byte[] pixelData)
		{
			Platform.CheckForNullReference(pixelData, "pixelData");
			_pixelData = pixelData;

			Initialize(rows, columns, bitsAllocated);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="PixelData"/> with the specified image parameters.
		/// </summary>
		/// <param name="rows">The number of rows.</param>
		/// <param name="columns">The number of columns.</param>
		/// <param name="bitsAllocated">The number of bits allocated in the pixel data returned by <paramref name="pixelDataGetter"/>.</param>
		/// <param name="pixelDataGetter">A delegate that returns the pixel data.</param>
		protected PixelData(
			int rows,
			int columns,
			int bitsAllocated,
			PixelDataGetter pixelDataGetter)
		{
			Platform.CheckForNullReference(pixelDataGetter, "pixelDataGetter");
			_pixelDataGetter = pixelDataGetter;

			Initialize(rows, columns, bitsAllocated);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gets the raw pixel data.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Use the byte array returned by this property to do unsafe processing.
		/// </para>
		/// <para>
		/// In general, you should avoid storing the byte array if at all possible.
		/// By storing it for say, the lifetime of an <see cref="ClearCanvas.ImageViewer.Graphics.ImageGraphic"/>, 
		/// future memory management schemes will be unable to release it, since
		/// a reference will have been created to it that such schemes may not
		/// be able to reach.  If you do need to store the byte array for some reason,
		/// do so only using local variables, since they have only method scope
		/// and can be garbage collected easily.
		/// </para>
		/// </remarks>
		public byte[] Raw
		{
			get 
			{
				return GetPixelData();
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Returns a copy of the object, including the pixel data.
		/// </summary>
		/// <returns></returns>
		public PixelData Clone()
		{
			return CloneInternal();
		}

		/// <summary>
		/// Gets the pixel value at the specified location.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <returns>
		/// The value of the pixel.  If the pixel data is colour,
		/// an ARGB value is returned, where A is the most significant byte.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
		public int GetPixel(int x, int y)
		{
			int i = GetIndex(x, y);
			return GetPixelInternal(i);
		}

		/// <summary>
		/// Gets the pixel value at the specific pixel index.
		/// </summary>
		/// <param name="pixelIndex">The index.</param>
		/// <remarks>
		/// If the pixel data is treated as a one-dimensional array
		/// where each row of pixels is concatenated, <paramref name="pixelIndex"/>
		/// is the index into that array.  This is useful when you know the
		/// index of the pixel that you want to get and don't want to 
		/// incur the needless computational overhead associated with specifying
		/// an x and y value.
		/// </remarks>
		public int GetPixel(int pixelIndex)
		{
			int i = pixelIndex * _bytesPerPixel;
			return GetPixelInternal(i);
		}

		/// <summary>
		/// Sets the pixel value at the specified location.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="value">The value of the pixel.  If the pixel
		/// data is colour, the value is in ARGB form, where A is the 
		/// most significant byte.</param>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds, or <paramref name="value"/>
		/// is out of range.</exception>
		public void SetPixel(int x, int y, int value)
		{
			int i = GetIndex(x, y);
			SetPixelInternal(i, value);
		}

		/// <summary>
		/// Sets the pixel value at the specified pixel index.
		/// </summary>
		/// <param name="pixelIndex">The pixel index.</param>
		/// <param name="value">The value of the pixel.  If the pixel
		/// data is colour, the value is in ARGB form, where A is the 
		/// most significant byte.</param>
		/// <remarks>
		/// If the pixel data is treated as a one-dimensional array
		/// where each row of pixels is concatenated, <paramref name="pixelIndex"/>
		/// is the index into that array.  This is useful when you know the
		/// index of the pixel that you want to set and don't want to 
		/// incur the needless computational overhead associated with specifying
		/// an x and y value.
		/// </remarks>
		public void SetPixel(int pixelIndex, int value)
		{
			int i = pixelIndex * _bytesPerPixel;
			SetPixelInternal(i, value);
		}

		/// <summary>
		/// Used in conjunction with <see cref="PixelData.ForEachPixel(int,int,int,int,PixelProcessor)"/> for pixel processing.
		/// </summary>
		/// <param name="i">The ith pixel processed so far.  This is a zero based index.
		/// If iterating over the entire image, <paramref name="i"/>
		/// is the same as <paramref name="pixelIndex"/>.</param>
		/// <param name="x">The x value of the current pixel being processed.</param>
		/// <param name="y">The y value of the current pixel being processed</param>
		/// <param name="pixelIndex">The index of the pixel being processed.</param>
		/// <remarks>
		/// It is often desirable to iterate through the pixels in an image, or
		/// the pixels in a rectangular region in an image so that some kind of
		/// processing can be done.  When used in conjunction with 
		/// <see cref="ForEachPixel(PixelProcessor)"/> or
		/// <see cref="ForEachPixel(int, int, int, int, PixelProcessor)"/>,
		/// this delegate allows the client to focus on writing the processing code,
		/// not the boilerplate iteration code.
		/// </remarks>
		public delegate void PixelProcessor(int i, int x, int y, int pixelIndex);

		/// <summary>
		/// Iterates through all the pixels in the image.
		/// </summary>
		/// <param name="processor">Called for each pixel.</param>
		/// <remarks>
		/// It is often desirable to iterate through all the pixels in an image.
		/// This method encapsulates all the boilerplate code required to do that.
		/// </remarks>
		public void ForEachPixel(PixelProcessor processor)
		{
			ForEachPixel(0, 0, _columns - 1, _rows - 1, processor);
		}

		/// <summary>
		/// Iterates through all the pixels in a rectangular region of the image.
		/// </summary>
		/// <param name="left">The left edge of the rectangular region.</param>
		/// <param name="top">The top edge of the rectangular region.</param>
		/// <param name="right">The right edge of the rectangular region.</param>
		/// <param name="bottom">The bottom edge of the rectangular region.</param>
		/// <param name="processor">The <see cref="PixelProcessor"/> delegate to call.</param>
		/// <remarks>
		/// It is often desirable to iterate through all the pixels in a rectangular
		/// region of an image. This method encapsulates all the boilerplate code 
		/// required to do that.
		/// </remarks>
		public void ForEachPixel(
			int left, int top, int right, int bottom, 
			PixelProcessor processor)
		{
			int i = 0;
			int temp;

			if (top > bottom)
			{
				temp = top;
				top = bottom;
				bottom = temp;
			}

			if (left > right)
			{
				temp = left;
				left = right;
				right = temp;
			}

			Platform.CheckArgumentRange(left, 0, _columns - 1, "left");
			Platform.CheckArgumentRange(right, 0, _columns - 1, "right");
			Platform.CheckArgumentRange(top, 0, _rows - 1, "top");
			Platform.CheckArgumentRange(bottom, 0, _rows - 1, "bottom");

			int pixelIndex = top * _columns + left;
			int offset = (_columns - right) + left - 1;

			for (int y = top; y <= bottom; y++)
			{
				for (int x = left; x <= right; x++)
				{
					processor(i, x, y, pixelIndex);
					pixelIndex++;
					i++;
				}

				pixelIndex += offset;
			}
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Returns a copy of the object, including the pixel data.
		/// </summary>
		/// <returns></returns>
		protected abstract PixelData CloneInternal();

		/// <summary>
		/// Gets the pixel value at the specified index.
		/// </summary>
		protected abstract int GetPixelInternal(int i);

		/// <summary>
		/// Sets the pixel at a specified index.
		/// </summary>
		protected abstract void SetPixelInternal(int i, int value);

		/// <summary>
		/// Gets the raw pixel data.
		/// </summary>
		protected byte[] GetPixelData()
		{
			if (_pixelData != null)
				return _pixelData;
			else
				return _pixelDataGetter();
		}

		#endregion

		#region Private methods

		private void Initialize(int rows, int columns, int bitsAllocated)
		{
			DicomValidator.ValidateRows(rows);
			DicomValidator.ValidateColumns(columns);
			_rows = rows;
			_columns = columns;
			_bitsAllocated = bitsAllocated;

			_bytesPerPixel = bitsAllocated / 8;
			_stride = _columns * _bytesPerPixel;
		}

		/// <summary>
		/// Gets the index in the pixel data array given the x and y
		/// location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected int GetIndex(int x, int y)
		{
			if (x < 0 ||
				x >= _columns ||
				y < 0 ||
				y >= _rows)
				throw new ArgumentException("x and/or y are out of bounds");

			int i = (y * _stride) + (x * _bytesPerPixel);
			return i;
		}

		#endregion
	}
}
