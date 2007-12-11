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

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
	/// A delegate that returns text for <paramref name="item"/> in a tree.
    /// </summary>
	public delegate string TextProviderDelegate<T>(T item);
	/// <summary>
	/// A delegate that returns whether or not <paramref name="item"/> is checked in a tree.
	/// </summary>
	public delegate bool IsCheckedGetterDelegate<T>(T item);
	/// <summary>
	/// A delegate that allows setting whether or not <paramref name="item"/> is checked in a tree.
	/// </summary>
	public delegate void IsCheckedSetterDelegate<T>(T item, bool value);
	/// <summary>
	/// A delegate that returns the icon set for <paramref name="item"/> in a tree.
	/// </summary>
    public delegate IconSet IconSetProviderDelegate<T>(T item);
	/// <summary>
	/// A delegate that returns an <see cref="IResourceResolver"/> for <paramref name="item"/> in a tree.
	/// </summary>
	public delegate IResourceResolver ResourceResolverProviderDelegate<T>(T item);
	/// <summary>
	/// A delegate that returns whether or not an <paramref name="item"/> in a tree can have a sub-tree.
	/// </summary>
	public delegate bool CanHaveSubTreeDelegate<T>(T item);
	/// <summary>
	/// A delegate that gets the <see cref="ITree"/> associated with <paramref name="item"/>.
	/// </summary>
    public delegate ITree TreeProviderDelegate<T>(T item);
	/// <summary>
	/// A delegate that gets whether or not the sub-tree should be expanded for <param name="item"/>.
	/// </summary>
    public delegate bool IsInitiallyExpandedDelegate<T>(T item);
	/// <summary>
	/// A delegate that determines whether or not <paramref name="item"/> can accept a dropped item.
	/// </summary>
	public delegate DragDropKind CanAcceptDropDelegate<T>(T item, object dropData, DragDropKind kind);
	/// <summary>
	/// A delegate that finalizes a drop operation.
	/// </summary>
	public delegate DragDropKind AcceptDropDelegate<T>(T item, object dropData, DragDropKind kind);

    /// <summary>
    /// A useful generic implementation of <see cref="ITreeItemBinding"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of item being bound to a tree.</typeparam>
    public class TreeItemBinding<TItem> : TreeItemBindingBase
    {
        private TextProviderDelegate<TItem> _nodeTextProvider;
        private IsCheckedGetterDelegate<TItem> _isCheckedGetter;
        private IsCheckedSetterDelegate<TItem> _isCheckedSetter;
        private TextProviderDelegate<TItem> _tooltipTextProvider;
        private IconSetProviderDelegate<TItem> _iconSetIndexProvider;
        private ResourceResolverProviderDelegate<TItem> _resourceResolverProvider;
        private CanHaveSubTreeDelegate<TItem> _canHaveSubTreeHandler;
        private IsInitiallyExpandedDelegate<TItem> _isInitiallyExpandedHandler;
        private TreeProviderDelegate<TItem> _subTreeProvider;
        private CanAcceptDropDelegate<TItem> _canAcceptDropHandler;
        private AcceptDropDelegate<TItem> _acceptDropHandler;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nodeTextProvider">A delegate providing text for the node in the tree.</param>
        /// <param name="subTreeProvider">A delegate providing the sub-tree for a node in the tree.</param>
        public TreeItemBinding(TextProviderDelegate<TItem> nodeTextProvider, TreeProviderDelegate<TItem> subTreeProvider)
        {
            _nodeTextProvider = nodeTextProvider;
            _subTreeProvider = subTreeProvider;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="nodeTextProvider">A delegate providing text for the node in the tree.</param>
        public TreeItemBinding(TextProviderDelegate<TItem> nodeTextProvider)
            : this(nodeTextProvider, null)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TreeItemBinding()
            :this(null, null)
        {
        }

        /// <summary>
        /// Gets or sets the node text provider for this binding.
        /// </summary>
        public TextProviderDelegate<TItem> NodeTextProvider
        {
            get { return _nodeTextProvider; }
            set { _nodeTextProvider = value; }
        }

        /// <summary>
        /// Gets or sets the node checked status provider for this binding.
        /// </summary>
        public IsCheckedGetterDelegate<TItem> IsCheckedGetter
        {
            get { return _isCheckedGetter; }
            set { _isCheckedGetter = value; }
        }

		/// <summary>
		/// Gets or sets the node checked setter for this binding.
		/// </summary>
		public IsCheckedSetterDelegate<TItem> IsCheckedSetter
        {
            get { return _isCheckedSetter; }
            set { _isCheckedSetter = value; }
        }

        /// <summary>
        /// Gets or sets the tooltip text provider for this binding.
        /// </summary>
        public TextProviderDelegate<TItem> TooltipTextProvider
        {
            get { return _tooltipTextProvider; }
            set { _tooltipTextProvider = value; }
        }

        /// <summary>
        /// Gets or sets the iconset provider for this binding.
        /// </summary>
        public IconSetProviderDelegate<TItem> IconSetProvider
        {
            get { return _iconSetIndexProvider; }
            set { _iconSetIndexProvider = value; }
        }

        /// <summary>
        /// Gets or sets the resource resolver provider for this binding.
        /// </summary>
        public ResourceResolverProviderDelegate<TItem> ResourceResolverProvider
        {
            get { return _resourceResolverProvider; }
            set { _resourceResolverProvider = value; }
        }

		/// <summary>
		/// Gets or sets the handler that determines whether or not this item can have a sub-tree.
		/// </summary>
        public CanHaveSubTreeDelegate<TItem> CanHaveSubTreeHandler
        {
            get { return _canHaveSubTreeHandler; }
            set { _canHaveSubTreeHandler = value; }
        }

        /// <summary>
        /// Gets or sets the subtree expansion state provider for this binding.
        /// </summary>
        public IsInitiallyExpandedDelegate<TItem> IsInitiallyExpandedHandler
        {
            get { return _isInitiallyExpandedHandler; }
            set { _isInitiallyExpandedHandler = value; }
        }

        /// <summary>
        /// Gets or sets the subtree provider for this binding.
        /// </summary>
        public TreeProviderDelegate<TItem> SubTreeProvider
        {
            get { return _subTreeProvider; }
            set { _subTreeProvider = value; }
        }

		/// <summary>
		/// Gets or sets the handler that decides whether or not an item can be dropped on this node in the tree.
		/// </summary>
        public CanAcceptDropDelegate<TItem> CanAcceptDropHandler
        {
            get { return _canAcceptDropHandler; }
            set { _canAcceptDropHandler = value; }
        }

		/// <summary>
		/// Gets or sets the handler that accepts dropped items onto this node in the tree.
		/// </summary>
		public AcceptDropDelegate<TItem> AcceptDropHandler
        {
            get { return _acceptDropHandler; }
            set { _acceptDropHandler = value; }
        }

    	///<summary>
    	/// Gets the text to display for the node representing the specified item.
    	///</summary>
    	public override string GetNodeText(object item)
        {
            return _nodeTextProvider((TItem)item);
        }

    	///<summary>
    	/// Gets whether or not <paramref name="item" /> is checked.
    	///</summary>
    	public override bool GetIsChecked(object item)
        {
            return _isCheckedGetter == null ? base.GetIsChecked(((TItem) item)) : _isCheckedGetter((TItem) item);
        }

    	///<summary>
    	/// Sets whether or not <paramref name="item" /> is checked.
    	///</summary>
    	public override void SetIsChecked(object item, bool value)
        {
            if(_isCheckedSetter != null)
            {
                _isCheckedSetter((TItem) item, value);
            }
        }

    	///<summary>
    	/// Asks if the item can have a subtree.
    	///</summary>
    	///<remarks>
    	/// Note that this method should return true to inidicate that it
    	/// is possible that the item might have a subtree.  This allows the view to determine whether to display
    	/// a "plus" sign next to the node, without having to actually call <see cref="M:ClearCanvas.Desktop.Trees.ITreeItemBinding.GetSubTree(System.Object)" />.
    	///</remarks>
    	public override bool CanHaveSubTree(object item)
        {
            return _canHaveSubTreeHandler == null ? base.CanHaveSubTree(item) : _canHaveSubTreeHandler((TItem)item);
        }

    	///<summary>
    	/// Gets the <see cref="T:ClearCanvas.Desktop.Trees.ITree" /> that represents the subtree for the specified item,
    	/// or null if the item does not have a subtree.
    	///</summary>
    	///<remarks>
    	/// Note that <see cref="M:ClearCanvas.Desktop.Trees.ITreeItemBinding.CanHaveSubTree(System.Object)" /> is called first,
    	/// and this method will be called only if <see cref="M:ClearCanvas.Desktop.Trees.ITreeItemBinding.CanHaveSubTree(System.Object)" /> returns true.
    	///</remarks>
    	public override ITree GetSubTree(object item)
        {
            return _subTreeProvider == null ? base.GetSubTree(item) : _subTreeProvider((TItem)item);
        }

    	///<summary>
    	/// Gets a value indicating if the item should be expanded when the tree is initially loaded.
    	///</summary>
    	public override bool IsInitiallyExpanded(object item)
        {
             return _isInitiallyExpandedHandler == null ? base.IsInitiallyExpanded(item) : _isInitiallyExpandedHandler((TItem)item);
        }

    	///<summary>
    	/// Gets the tooltip to display for the specified item.
    	///</summary>
    	public override string GetTooltipText(object item)
        {
            return _tooltipTextProvider == null ? base.GetTooltipText(item) : _tooltipTextProvider((TItem)item);
        }

    	///<summary>
    	/// Gets the image iconset to display for the specified item.
    	///</summary>
    	public override IconSet GetIconSet(object item)
        {
            return _iconSetIndexProvider == null ? base.GetIconSet(item) : _iconSetIndexProvider((TItem)item);
        }

    	///<summary>
    	/// Gets the resource resolver used to resolve the icon(s).
    	///</summary>
    	public override IResourceResolver GetResourceResolver(object item)
        {
            return _resourceResolverProvider == null ? base.GetResourceResolver(item) : _resourceResolverProvider((TItem)item);
        }

    	///<summary>
    	/// Asks the specified item if it can accept the specified drop data in a drag-drop operation.
    	///</summary>
    	///<param name="item">The item being drag-dropped.</param>
    	///<param name="dropData">Information about the item drag-dropped.</param>
    	///<param name="kind">The drop kind being performed.</param>
    	///<returns>
    	///The drop kind that will be accepted.
    	///</returns>
    	public override DragDropKind CanAcceptDrop(object item, object dropData, DragDropKind kind)
        {
            return _canAcceptDropHandler == null ? base.CanAcceptDrop(item, dropData, kind) : _canAcceptDropHandler((TItem)item, dropData, kind);
        }

    	///<summary>
    	/// Informs the specified item that it should accept a drop of the specified data, completing a drag-drop operation.
    	///</summary>
    	///<param name="item">The item being drag-dropped.</param>
    	///<param name="dropData">Information about the item being drag-dropped.</param>
    	///<param name="kind">The drop kind being performed.</param>
    	///<returns>
    	///The drop kind that will be accepted.
    	///</returns>
    	public override DragDropKind AcceptDrop(object item, object dropData, DragDropKind kind)
        {
            return _acceptDropHandler == null ? base.AcceptDrop(item, dropData, kind) : _acceptDropHandler((TItem)item, dropData, kind);
        }
    }
}
