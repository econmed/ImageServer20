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

using System;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
    public interface IFolderSystem : IDisposable
    {
        string Id { get; }
        string DisplayName { get; }
        IList<IFolder> Folders { get; }
        IToolSet FolderTools { get; }
        IToolSet ItemTools { get; }
        string PreviewUrl { get; }

        void SelectedFolderChangedEventHandler(object sender, EventArgs e);
        void SelectedItemsChangedEventHandler(object sender, EventArgs e);
        void SelectedItemDoubleClickedEventHandler(object sender, EventArgs e);
    }

    public abstract class WorkflowFolderSystem<TItem> : IFolderSystem
    {
        private readonly IFolderExplorerToolContext _folderExplorer;
        private readonly IDictionary<string, Type> _worklistType;
        private readonly IList<IFolder> _workflowFolders;

        private event EventHandler _selectedItemDoubleClicked;
        private event EventHandler _selectedItemsChanged;
        private event EventHandler _selectedFolderChanged;

        protected IToolSet _itemTools;
        protected IToolSet _folderTools;

        public WorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
            : this(folderExplorer, null)
        {
        }

        public WorkflowFolderSystem(IFolderExplorerToolContext folderExplorer, ExtensionPoint<IFolder> folderExtensionPoint)
        {
            _workflowFolders = new List<IFolder>();

            _folderExplorer = folderExplorer;

            // Collect all worklist tokens
            _worklistType = new Dictionary<string, Type>();
            if (folderExtensionPoint != null)
            {
                foreach (IFolder folder in folderExtensionPoint.CreateExtensions())
                {
                    if (folder is WorkflowFolder<TItem>)
                    {
                        WorkflowFolder<TItem> workflowFolder = (WorkflowFolder<TItem>)folder;
                        if (!string.IsNullOrEmpty(workflowFolder.WorklistType))
                            _worklistType.Add(workflowFolder.WorklistType, workflowFolder.GetType());
                    }
                }
            }

            _itemTools = new ToolSet();
            _folderTools = new ToolSet();
        }

        ~WorkflowFolderSystem()
        {
            Dispose(false);
        }

        #region IFolderSystem implmentation

        public string Id
        {
            get { return this.GetType().FullName; }
        }

        public abstract string DisplayName { get; }

        public IList<IFolder> Folders
        {
            get { return _workflowFolders; }
        }

        public IToolSet FolderTools
        {
            get { return _folderTools; }
        }

        public IToolSet ItemTools
        {
            get { return _itemTools; }
        }

        public abstract string PreviewUrl { get; }

        #endregion

        /// <summary>
        /// Invalidates all folders of the specified type so that they
        /// will refresh their content or count.
        /// </summary>
        /// <param name="folderType"></param>
        public void InvalidateFolder(Type folderType)
        {
            // TODO: could implement more of an "invalidate", rather than an immediate refresh
            // the folder doesn't actually need to refresh unless the explorer workspace is visible
            foreach (IFolder folder in _workflowFolders)
            {
                if(folder.GetType().Equals(folderType))
                {
                    if(folder.IsOpen)
                        folder.Refresh();
                    else 
                        folder.RefreshCount();
                }
            }
        }

        /// <summary>
        /// Invalidates the currently selected folder, causing it to re-populate its contents.
        /// </summary>
        public void InvalidateSelectedFolder()
        {
            // TODO: could implement more of an "invalidate", rather than an immediate refresh
            // the folder doesn't actually need to refresh unless the explorer workspace is visible
            _folderExplorer.SelectedFolder.Refresh();
        }

        public Type GetWorklistType(string type)
        {
            return _worklistType.ContainsKey(type) ? _worklistType[type] : null;
        }

        public List<string> WorklistTokens
        {
            get { return new List<string>(_worklistType.Keys); }
        }

        public IDesktopWindow DesktopWindow
        {
            get { return _folderExplorer.DesktopWindow; }
        }

        public event EventHandler SelectedItemDoubleClicked
        {
            add { _selectedItemDoubleClicked += value; }
            remove { _selectedItemDoubleClicked -= value; }
        }

        public event EventHandler SelectedItemsChanged
        {
            add { _selectedItemsChanged += value; }
            remove { _selectedItemsChanged -= value; }
        }

        public event EventHandler SelectedFolderChanged
        {
            add { _selectedFolderChanged += value; }
            remove { _selectedFolderChanged -= value; }
        }

        public IFolder SelectedFolder
        {
            get { return _folderExplorer.SelectedFolder; }
            set { _folderExplorer.SelectedFolder = value; }
        }

        public ISelection SelectedItems
        {
            get { return _folderExplorer.SelectedItems; }
        }

        public virtual void SelectedFolderChangedEventHandler(object sender, EventArgs e)
        {
            EventsHelper.Fire(_selectedFolderChanged, this, EventArgs.Empty);
        }

        public virtual void SelectedItemsChangedEventHandler(object sender, EventArgs e)
        {
            EventsHelper.Fire(_selectedItemsChanged, this, EventArgs.Empty);
        }

        public virtual void SelectedItemDoubleClickedEventHandler(object sender, EventArgs e)
        {
            EventsHelper.Fire(_selectedItemDoubleClicked, this, EventArgs.Empty);
        }

        protected void AddFolder(WorkflowFolder<TItem> folder)
        {
            _workflowFolders.Add(folder);
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (WorkflowFolder<TItem> folder in _workflowFolders)
            {
                folder.Dispose();
            }

            if (disposing)
            {
                if (_itemTools != null) _itemTools.Dispose();
                if (_folderTools != null) _folderTools.Dispose();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
