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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions
{
	[ExtensionPoint]
	public class ListFilterMenuActionViewExtensionPoint : ExtensionPoint<IActionView> {}

	public interface IListFilterDataSource
	{
		IEnumerable<object> Values { get; }
		bool GetSelectedState(object value);
		void SetSelectedState(object value, bool selected);
		void SetAllSelectedState(bool selected);
	}

	[AssociateView(typeof (ListFilterMenuActionViewExtensionPoint))]
	public class ListFilterMenuAction : Action
	{
		private readonly IListFilterDataSource _dataSource;

		public ListFilterMenuAction(string actionID, ActionPath actionPath, IListFilterDataSource dataSource, IResourceResolver resourceResolver)
			: base(actionID, actionPath, resourceResolver)
		{
			Platform.CheckForNullReference(dataSource, "dataSource");
			_dataSource = dataSource;
		}

		public IListFilterDataSource DataSource
		{
			get { return _dataSource; }
		}

		public static ListFilterMenuAction CreateAction(Type callingType, string actionID, string actionPath, IListFilterDataSource dataSource, IResourceResolver resourceResolver)
		{
			ListFilterMenuAction action = new ListFilterMenuAction(
				string.Format("{0}:{1}", callingType.FullName, actionID),
				new ActionPath(actionPath, resourceResolver),
				dataSource, resourceResolver);
			action.Label = action.Path.LastSegment.LocalizedText;
			action.Persistent = true;
			return action;
		}
	}
}