using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract base class for collections of <see cref="DesktopObject"/> subclasses.
    /// </summary>
    public abstract class DesktopObjectCollection
    {
    }

    /// <summary>
    /// Generic abstract base class for collections of <see cref="DesktopObject"/> subclasses.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="DesktopObject"/> subclass.</typeparam>
    public abstract class DesktopObjectCollection<T> : DesktopObjectCollection, IEnumerable<T>, IDisposable
        where T : DesktopObject
    {

        private List<T> _list;
        private Dictionary<string, T> _nameMap;

        private event EventHandler<ItemEventArgs<T>> _itemOpening;
        private event EventHandler<ItemEventArgs<T>> _itemOpened;
        private event EventHandler<ClosingItemEventArgs<T>> _itemClosing;
        private event EventHandler<ClosedItemEventArgs<T>> _itemClosed;
        private event EventHandler<ItemEventArgs<T>> _itemActivationChanged;
        private event EventHandler<ItemEventArgs<T>> _itemVisibilityChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected DesktopObjectCollection()
        {
            _list = new List<T>();
            _nameMap = new Dictionary<string, T>();
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~DesktopObjectCollection()
        {
            Dispose(false);
        }

        #region Public properties

        /// <summary>
        /// Gets the object in the collection with the specified name.
        /// </summary>
        /// <param name="name">The name of the desktop object</param>
        /// <returns></returns>
        public T this[string name]
        {
            get
            {
                return _nameMap[name];
            }
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Checks if the specified item exists in this collection.
        /// </summary>
        /// <param name="obj">The desktop object to look for.</param>
        /// <returns></returns>
        public bool Contains(T obj)
        {
            return _list.Contains(obj);
        }

        #endregion

        #region Public events

        /// <summary>
        /// Occurs when a new item is about to open, after it has been added to the collection.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemOpening
        {
            add { _itemOpening += value; }
            remove { _itemOpening -= value; }
        }

        /// <summary>
        /// Occurs after a new item has opened.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemOpened
        {
            add { _itemOpened += value; }
            remove { _itemOpened -= value; }
        }

        /// <summary>
        /// Occurs before an item is about to close.
        /// </summary>
        public event EventHandler<ClosingItemEventArgs<T>> ItemClosing
        {
            add { _itemClosing += value; }
            remove { _itemClosing -= value; }
        }
        
        /// <summary>
        /// Occurs after an item has closed and been removed from the collection. 
        /// </summary>
        public event EventHandler<ClosedItemEventArgs<T>> ItemClosed
        {
            add { _itemClosed += value; }
            remove { _itemClosed -= value; }
        }

        /// <summary>
        /// Occurs when the <see cref="DesktopObject.Visible"/> property of an item in the collection changes.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemVisibilityChanged
        {
            add { _itemVisibilityChanged += value; }
            remove { _itemVisibilityChanged -= value; }
        }

        /// <summary>
        /// Occurs when the <see cref="DesktopObject.Active"/> property of an item in the collection changes.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemActivationChanged
        {
            add { _itemActivationChanged += value; }
            remove { _itemActivationChanged -= value; }
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                // shouldn't throw anything from inside Dispose()
                Platform.Log(e);
            }
        }

        #endregion

        #region Protected overridables

        /// <summary>
        /// Raises the <see cref="ItemOpening"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnItemOpening(ItemEventArgs<T> args)
        {
            EventsHelper.Fire(_itemOpening, this, args);
        }

        /// <summary>
        /// Raises the <see cref="ItemOpened"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnItemOpened(ItemEventArgs<T> args)
        {
            EventsHelper.Fire(_itemOpened, this, args);
        }

        /// <summary>
        /// Raises the <see cref="ItemClosing"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnItemClosing(ClosingItemEventArgs<T> args)
        {
            EventsHelper.Fire(_itemClosing, this, args);
        }

        /// <summary>
        /// Raises the <see cref="ItemClosed"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnItemClosed(ClosedItemEventArgs<T> args)
        {
            EventsHelper.Fire(_itemClosed, this, args);
        }

        /// <summary>
        /// Raises the <see cref="ItemVisibilityChanged"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnItemVisibilityChanged(ItemEventArgs<T> args)
        {
            EventsHelper.Fire(_itemVisibilityChanged, this, args);
        }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnItemActivationChangedInternal(ItemEventArgs<T> args)
        {
            // default behaviour, just raise the public event
            args.Item.RaiseActiveChanged();
        }

        /// <summary>
        /// Raises the <see cref="ItemActivationChanged"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnItemActivationChanged(ItemEventArgs<T> args)
        {
            EventsHelper.Fire(_itemActivationChanged, this, args);
        }

        /// <summary>
        /// Disposes of this collection, first disposing of each object in the collection.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (T obj in _list)
                {
                    try
                    {
                        (obj as IDisposable).Dispose();
                    }
                    catch (Exception e)
                    {
                        Platform.Log(e);
                    }
                }

                _nameMap.Clear();
                _list.Clear();
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Opens the specified object.
        /// </summary>
        /// <param name="obj"></param>
        protected void Open(T obj)
        {
            obj.Opening += delegate(object sender, EventArgs e)
            {
                Add((T)sender);
                OnItemOpening(new ItemEventArgs<T>((T)sender));
            };
            obj.Opened += delegate(object sender, EventArgs e)
            {
                OnItemOpened(new ItemEventArgs<T>((T)sender));
            };
            obj.Closing += delegate(object sender, ClosingEventArgs e)
            {
                ClosingItemEventArgs<T> args = new ClosingItemEventArgs<T>((T)sender, e.Reason, e.Interaction, e.Cancel);
                OnItemClosing(args);
                e.Cancel = args.Cancel;
            };
            obj.Closed += delegate(object sender, ClosedEventArgs e)
            {
                Remove((T)sender);
                OnItemClosed(new ClosedItemEventArgs<T>((T)sender, e.Reason));
            };
            obj.VisibleChanged += delegate(object sender, EventArgs e)
            {
                OnItemVisibilityChanged(new ItemEventArgs<T>((T)sender));
            };
            obj.InternalActiveChanged += delegate(object sender, EventArgs e)
            {
                OnItemActivationChangedInternal(new ItemEventArgs<T>((T)sender));
            };
            obj.ActiveChanged += delegate(object sender, EventArgs e)
            {
                OnItemActivationChanged(new ItemEventArgs<T>((T)sender));
            };

            obj.Open();
        }

        /// <summary>
        /// Adds the specified object to the collection.
        /// </summary>
        /// <param name="obj"></param>
        private void Add(T obj)
        {
            if (_list.Contains(obj))
                throw new InvalidOperationException(SR.ExceptionObjectAlreadyInCollection);
            if (obj.Name != null && _nameMap.ContainsKey(obj.Name))
                throw new InvalidOperationException(string.Format(SR.ExceptionObjectWithNameAlreadyExists, obj.Name));

            _list.Add(obj);
            if (obj.Name != null)
            {
                _nameMap.Add(obj.Name, obj);
            }
        }

        /// <summary>
        /// Removes the specified object from the collection.
        /// </summary>
        /// <param name="obj"></param>
        private void Remove(T obj)
        {
            if (_nameMap.ContainsValue(obj))
                _nameMap.Remove(obj.Name);
            _list.Remove(obj);
        }

        #endregion
    }
}
