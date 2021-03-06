﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A collection of <see cref="Frame"/> objects.
	/// </summary>
	public class FrameCollection : IEnumerable<Frame>
	{
		private readonly List<Frame> _frames = new List<Frame>();

		internal FrameCollection()
		{
		}

		/// <summary>
		/// Gets the number of <see cref="Frame"/> objects in the collection.
		/// </summary>
		public int Count
		{
			get { return _frames.Count; }
		}

		/// <summary>
		/// Gets the <see cref="Frame"/> at the specified index.
		/// </summary>
		/// <param name="frameNumber">The frame number. The first frame is frame 1.</param>
		/// <returns></returns>
		public Frame this[int frameNumber]
		{
			get
			{
				Platform.CheckPositive(frameNumber, "frameNumber");
				return _frames[frameNumber-1];
			}
		}

		/// <summary>
		/// Adds a <see cref="Frame"/> to the collection.
		/// </summary>
		/// <param name="frame"></param>
		/// <remarks>
		/// This method should only be used by subclasses of <see cref="ImageSop"/>.
		/// </remarks>
		public void Add(Frame frame)
		{
			_frames.Add(frame);
		}

		#region IEnumerable<Frame> Members

		///<summary>
		///Returns an enumerator that iterates through the collection.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>1</filterpriority>
		public IEnumerator<Frame> GetEnumerator()
		{
			return _frames.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		///<summary>
		///Returns an enumerator that iterates through a collection.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _frames.GetEnumerator();
		}

		#endregion
	}
}
