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
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A static factory class which creates <see cref="IPresentationImage"/>s.
	/// </summary>
	public static class PresentationImageFactory
	{
		/// <summary>
		/// Creates an appropriate subclass of <see cref="BasicPresentationImage"/>
		/// based on the <see cref="ImageSop"/>'s photometric interpretation.
		/// </summary>
		/// <param name="imageSop"></param>
		/// <returns></returns>
		public static IEnumerable<IPresentationImage> Create(ImageSop imageSop)
		{
			List<IPresentationImage> list = new List<IPresentationImage>();

			foreach (Frame frame in imageSop.Frames)
			{
				if (frame.PhotometricInterpretation == PhotometricInterpretation.Unknown)
				{
					throw new Exception("Photometric interpretation is unknown.");
				}
				else if (frame.PhotometricInterpretation == PhotometricInterpretation.Monochrome1 ||
						 frame.PhotometricInterpretation == PhotometricInterpretation.Monochrome2)
				{
					list.Add(new DicomGrayscalePresentationImage(frame));
				}
				else
				{
					list.Add(new DicomColorPresentationImage(frame));
				}
			}

			return list;
		}
	}
}
