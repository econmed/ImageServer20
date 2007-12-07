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

using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	internal sealed class StoredAnnotationLayout : AnnotationLayout
	{
		private string _identifier;
		private List<StoredAnnotationBoxGroup> _annotationBoxGroups;

		public StoredAnnotationLayout(string identifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			_identifier = identifier;
			_annotationBoxGroups = new List<StoredAnnotationBoxGroup>();
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public StoredAnnotationBoxGroup this [string groupId]
		{
			get
			{
				return _annotationBoxGroups.Find(delegate(StoredAnnotationBoxGroup group){ return group.Identifier == groupId; });
			}
		}

		public IList<StoredAnnotationBoxGroup> AnnotationBoxGroups
		{
			get { return _annotationBoxGroups; }
		}

		#region IAnnotationLayout

		public override IEnumerable<AnnotationBox> AnnotationBoxes
		{
			get
			{
				foreach (StoredAnnotationBoxGroup group in _annotationBoxGroups)
				{
					foreach (AnnotationBox box in group.AnnotationBoxes)
						yield return box;
				}
			}
		}

		#endregion
	}
}
