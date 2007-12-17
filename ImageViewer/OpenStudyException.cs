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

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// The exception that is thrown when a study or image fails to open.
	/// </summary>
	public class OpenStudyException : Exception
	{
		private int _totalImages;
		private int _failedImages;

		/// <summary>
		/// Instantiates a new instance of <see cref="OpenStudyException"/>.
		/// </summary>
		public OpenStudyException() { }

		/// <summary>
		/// Instantiates a new instance of <see cref="OpenStudyException"/> with
		/// a specified message.
		/// </summary>
		public OpenStudyException(string message) : base(message) { }

		/// <summary>
		/// Instantiates a new instance of <see cref="OpenStudyException"/> with
		/// a specified message and inner exception.
		/// </summary>
		public OpenStudyException(string message, Exception inner) : base(message, inner) { }

		/// <summary>
		/// Gets or sets the total number of images the Framework tried to open.
		/// </summary>
		public int TotalImages
		{
			get { return _totalImages; }
			set { _totalImages = value; }
		}

		/// <summary>
		/// Gets or sets the number of images that failed to open.
		/// </summary>
		public int FailedImages
		{
			get { return _failedImages; }
			set { _failedImages = value; }
		}

		/// <summary>
		/// Gets the number of images that open successfully.
		/// </summary>
		public int SuccessfulImages
		{
			get { return this.TotalImages - this.FailedImages; }
		}
	}
}
