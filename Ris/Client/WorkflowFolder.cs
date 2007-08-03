using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class WorkflowFolderExtensionPoint : ExtensionPoint<IFolder>
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FolderForWorklistTypeAttribute : Attribute
    {
        private readonly string _worklistType;

        public FolderForWorklistTypeAttribute(string worklistType)
        {
            _worklistType = worklistType;
        }

        public string WorklistType
        {
            get { return _worklistType; }
        }
    }

    public abstract class WorkflowFolder<TItem> : Folder, IDisposable
    {
        private string _folderName;
        private string _folderTooltip;
        private Table<TItem> _itemsTable;
        private bool _isPopulated;
        private int _itemCount = -1;
        private WorkflowFolderSystem<TItem> _folderSystem;

        private Timer _refreshTimer;
        private int _refreshTime;

        private ExtensionPoint<IDropHandler<TItem>> _dropHandlerExtensionPoint;
        private IDropContext _dropContext;
        private IDropHandler<TItem> _currentDropHandler;

        private BackgroundTask _queryItemsTask;
        private BackgroundTask _queryCountTask;

        public WorkflowFolder(WorkflowFolderSystem<TItem> folderSystem, string folderName, string folderTooltip, Table<TItem> itemsTable)
        {
            _folderSystem = folderSystem;
            _folderName = folderName;
            _folderTooltip = folderTooltip;
            _itemsTable = itemsTable;
            _itemsTable.Items.ItemsChanged += delegate(object sender, ItemChangedEventArgs args)
                {
                    _itemCount = _itemsTable.Items.Count;
                    NotifyTextChanged();
                };
        }

        public void UpdateWorklistItem(TItem item)
        {
            // if the folder has not yet been populated, then nothing to do
            if (!_isPopulated)
                return;

            // get the index of the item in this folder, if it exists
            int i = _itemsTable.Items.FindIndex(delegate(TItem x) { return x.Equals(item); });

            // is the item a member of this folder?
            if (IsMember(item))
            {
                if (i > -1)
                {
                    // update the item that is already contained in this folder
                    _itemsTable.Items[i] = item;
                }
                else
                {
                    // add the item, because it was not already contained in this folder
                    _itemsTable.Items.Add(item);
                }
            }
            else
            {
                if (i > -1)
                {
                    // remove the item from this folder, because it is no longer a member
                    _itemsTable.Items.RemoveAt(i);
                }
            }
        }

        public override string Text
        {
            get
            {
                return _isPopulated || _itemCount >= 0 ?
                    string.Format("{0} ({1})", _folderName, _itemCount) : _folderName;
            }
        }

        public override string Tooltip
        {
            get
            {
                return _folderTooltip;
            }
        }

        public int ItemCount
        {
            get { return _itemCount; }
            set
            {
                _itemCount = value;
                NotifyTextChanged();
            }
        }

        public override ITable ItemsTable
        {
            get
            {
                return _itemsTable;
            }
        }

        public WorkflowFolderSystem<TItem> WorkflowFolderSystem
        {
            get { return _folderSystem; }
        }

        public int RefreshTime
        {
            get { return _refreshTime; }
            set
            {
                _refreshTime = value;
                this.RestartRefreshTimer();
            }
        }

        public override void  Refresh()
        {
            if (_queryItemsTask != null)
            {
                // refresh already in progress
                return;
            }

            if (CanQuery())
            {
                _queryItemsTask = new BackgroundTask(
                    delegate(IBackgroundTaskContext taskContext)
                    {
                        try
                        {
                            IList<TItem> items = QueryItems();
                            taskContext.Complete(items);
                        }
                        catch (Exception e)
                        {
                            taskContext.Error(e);
                        }
                    },
                    false);

                _queryItemsTask.Terminated += OnQueryItemsCompleted;
                _queryItemsTask.Run();
            }
        }

        private void OnQueryItemsCompleted(object sender, BackgroundTaskTerminatedEventArgs args)
        {
            if(args.Reason == BackgroundTaskTerminatedReason.Completed)
            {
                NotifyRefreshBegin();

                IList<TItem> items = (IList<TItem>)args.Result;
                _isPopulated = true;
                _itemsTable.Items.Clear();
                _itemsTable.Items.AddRange(items);
                _itemsTable.Sort();

                NotifyRefreshFinish();
            }
            else
            {
                Platform.Log(LogLevel.Error, args.Exception);
            }

            // dispose of the task
            _queryItemsTask.Terminated -= OnQueryItemsCompleted;
            _queryItemsTask.Dispose();
            _queryItemsTask = null;

            this.RestartRefreshTimer();
        }

        public override void RefreshCount()
        {
            if (_queryCountTask != null)
            {
                // refresh already in progress
                return;
            }

            if (CanQuery())
            {
                _queryCountTask = new BackgroundTask(
                    delegate(IBackgroundTaskContext taskContext)
                    {
                        try
                        {
                            int count = QueryCount();
                            taskContext.Complete(count);
                        }
                        catch (Exception e)
                        {
                            taskContext.Error(e);
                        }
                    },
                    false);

                _queryCountTask.Terminated += OnQueryCountCompleted;
                _queryCountTask.Run();

            }
        }

        private void OnQueryCountCompleted(object sender, BackgroundTaskTerminatedEventArgs args)
        {
            if (args.Reason == BackgroundTaskTerminatedReason.Completed)
            {
                this.ItemCount = (int)args.Result;
            }
            else
            {
                Platform.Log(LogLevel.Error, args.Exception);
            }

            // dispose of the task
            _queryCountTask.Terminated -= OnQueryCountCompleted;
            _queryCountTask.Dispose();
            _queryCountTask = null;

            this.RestartRefreshTimer();
        }

        public override void OpenFolder()
        {
            base.OpenFolder();

            this.RestartRefreshTimer();
        }

        public override void CloseFolder()
        {
            base.CloseFolder();

            this.RestartRefreshTimer();
        }

        public override void DragComplete(object[] items, DragDropKind kind)
        {
            if (kind == DragDropKind.Move)
            {
                // items have been "moved" out of this folder
            }
        }

        public override DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
        {
            if (_dropHandlerExtensionPoint == null)
                return DragDropKind.None;

            // cast items to type safe collection
            ICollection<TItem> dropItems = CollectionUtils.Map<object, TItem>(items, delegate(object item) { return (TItem)item; });

            // check for a handler that can accept
            _currentDropHandler = CollectionUtils.SelectFirst<IDropHandler<TItem>>(_dropHandlerExtensionPoint.CreateExtensions(),
                delegate(IDropHandler<TItem> handler)
                {
                    return handler.CanAcceptDrop(_dropContext, dropItems);
                });

            // if the items are acceptable, return Move (never Copy, which would make no sense for a workflow folder)
            return _currentDropHandler != null ? DragDropKind.Move : DragDropKind.None;
        }

        public override DragDropKind AcceptDrop(object[] items, DragDropKind kind)
        {
            if (_currentDropHandler == null)
                return DragDropKind.None;

            // cast items to type safe collection
            ICollection<TItem> dropItems = CollectionUtils.Map<object, TItem>(items, delegate(object item) { return (TItem)item; });
            return _currentDropHandler.ProcessDrop(_dropContext, dropItems) ? DragDropKind.Move : DragDropKind.None;
        }

        protected void RestartRefreshTimer()
        {
            if (_refreshTimer != null)
            {
                _refreshTimer.Stop();
                _refreshTimer.Dispose();
                _refreshTimer = null;
            }

            if (_refreshTime > 0)
            {
                TimerDelegate timerDelegate = this.IsOpen
                    ? new TimerDelegate(delegate(object state) { Refresh(); })
                    : new TimerDelegate(delegate(object state) { RefreshCount(); });

                _refreshTimer = new Timer(timerDelegate);
                _refreshTimer.Interval = _refreshTime;
                _refreshTimer.Start();
            }
        }

        protected void InitDragDropHandling(ExtensionPoint<IDropHandler<TItem>> dropHandlerExtensionPoint, IDropContext dropContext)
        {
            _dropHandlerExtensionPoint = dropHandlerExtensionPoint;
            _dropContext = dropContext;
        }

        protected abstract bool CanQuery();
        protected abstract int QueryCount();
        protected abstract IList<TItem> QueryItems();
        protected abstract bool IsMember(TItem item);

        #region IDisposable Members

        public void Dispose()
        {
            if (_refreshTimer != null)
            {
                _refreshTimer.Dispose();
                _refreshTimer = null;
            }
        }

        #endregion
    }
}
