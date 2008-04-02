﻿#region License

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

using System.Drawing;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class PaletteColorMap : ColorMap
	{
		private int _size;
		private int _bitsPerLutEntry;
		private byte[] _redLut;
		private byte[] _greenLut;
		private byte[] _blueLut;

		public PaletteColorMap(
			int size,
			int firstMappedPixel,
			int bitsPerLutEntry,
			byte[] redLut,
			byte[] greenLut,
			byte[] blueLut)
		{
			_size = size;
			this.MinInputValue = firstMappedPixel;
			this.MaxInputValue = firstMappedPixel + size - 1;
			_bitsPerLutEntry = bitsPerLutEntry;
			_redLut = redLut;
			_greenLut = greenLut;
			_blueLut = blueLut;
		}

		protected override void Create()
		{
			Color color;

			if (_bitsPerLutEntry == 8)
			{
				// Account for case where an 8-bit entry is encoded in a 16 bits allocated
				// i.e., 8 bits of padding per entry
				if (_redLut.Length == 2 * _size)
				{
					int offset = 0;
					for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
					{
						// Get the low byte of the 16-bit entry
						color = Color.FromArgb(255, _redLut[offset], _greenLut[offset], _blueLut[offset]);
						this[i] = color.ToArgb();
						offset += 2;
					}
				}
				else
				{
					// The regular 8-bit case
					int offset = 0;
					for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
					{
						color = Color.FromArgb(255, _redLut[offset], _greenLut[offset], _blueLut[offset]);
						this[i] = color.ToArgb();
						++offset;
					}
				}
			}
			// 16 bit entries
			else
			{
				int offset = 1;
				for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
				{
					// Just get the high byte, since we'd have to right shift the
					// 16-bit value by 8 bits to scale it to an 8 bit value anyway.
					color = Color.FromArgb(255, _redLut[offset], _greenLut[offset], _blueLut[offset]);
					this[i] = color.ToArgb();

					offset += 2;
				}
			}
		}

		public override string GetDescription()
		{
			return SR.DescriptionPaletteColorMap;
		}
	}
}
