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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Comparers;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="IDisplaySet"/> objects.
	/// </summary>
	public class DisplaySetCollection : ObservableList<IDisplaySet>
	{
		private IComparer<IDisplaySet> _comparer;

		/// <summary>
		/// Instantiates a new instance of <see cref="DisplaySetCollection"/>.
		/// </summary>
		internal DisplaySetCollection()
		{

		}

		internal static IComparer<IDisplaySet> GetDefaultComparer()
		{
			return new DefaultDisplaySetComparer();
		}

		internal IComparer<IDisplaySet> Comparer
		{
			get { return _comparer; }
			set { _comparer = value; }
		}

		public void Sort()
		{
			Sort(_comparer ?? GetDefaultComparer());
		}

		/// <summary>
		/// Sorts the collection with the given comparer.
		/// </summary>
		public override void Sort(IComparer<IDisplaySet> comparer)
		{
			Platform.CheckForNullReference(comparer, "comparer");
			_comparer = comparer;
			base.Sort(_comparer);
		}
	}
}
