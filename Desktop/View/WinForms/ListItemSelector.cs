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
using System.Windows.Forms;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class ListItemSelector : UserControl
    {
        #region Private Members

        private ITable _availableItemsTable = null;
        private ITable _selectedItemsTable = null;

        private event EventHandler _itemAdded;
        private event EventHandler _itemRemoved;

        #endregion

        #region Constructor

        public ListItemSelector()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Properties

        public ITable AvailableItemsTable
        {
            get { return _availableItemsTable; }
            set
            {
                _availableItemsTable = value;
                _availableItems.Table = _availableItemsTable;

                if(_availableItemsTable != null)
                {
                    _availableItemsTable.Sort();
                } 
            }
        }

        public ITable SelectedItemsTable
        {
            get { return _selectedItemsTable; }
            set
            {
                _selectedItemsTable = value;
                _selectedItems.Table = _selectedItemsTable;

                if(_selectedItemsTable != null) _selectedItemsTable.Sort();
            }
        }

        public void OnAvailableItemsChanged(object sender, EventArgs args)
        {
            if(_availableItemsTable.IsFiltered) _availableItemsTable.Filter();
            _availableItems.Table = _availableItemsTable;
        }

        #endregion

        #region Public Events

        public event EventHandler ItemAdded
        {
            add { _itemAdded += value; }
            remove { _itemAdded -= value; }
        }

        public event EventHandler ItemRemoved
        {
            add { _itemRemoved += value; }
            remove { _itemRemoved -= value; }
        }

        #endregion

        #region Private Methods

        private void AddSelection(object sender, EventArgs e)
        {
            ISelection selection = _availableItems.Selection;
            ISelection originalSelectedSelection = _selectedItems.Selection;
            ISelection availableSelectionAfterRemove = GetSelectionAfterRemove(_availableItemsTable, selection);

            foreach (object item in selection.Items)
            {
                _selectedItemsTable.Items.Add(item);
                _availableItemsTable.Items.Remove(item);
            }

            _selectedItemsTable.Sort();

            _selectedItems.Table = _selectedItemsTable;
            _availableItems.Table = _availableItemsTable;

            _selectedItems.Selection = originalSelectedSelection;
            _availableItems.Selection = availableSelectionAfterRemove;

            EventsHelper.Fire(_itemAdded, this, EventArgs.Empty);
        }

        private void RemoveSelection(object sender, EventArgs e)
        {
            ISelection selection = _selectedItems.Selection;
            ISelection originalAvailableSelection = _availableItems.Selection;
            ISelection selectedSelectionAfterRemove = GetSelectionAfterRemove(_selectedItemsTable, selection);

            foreach (object item in selection.Items)
            {
                _selectedItemsTable.Items.Remove(item);
                _availableItemsTable.Items.Add(item);
            }

            _availableItemsTable.Sort();

            _selectedItems.Table = _selectedItemsTable;
            _availableItems.Table = _availableItemsTable;

            _availableItems.Selection = originalAvailableSelection;
            _selectedItems.Selection = selectedSelectionAfterRemove;

            EventsHelper.Fire(_itemRemoved, this, EventArgs.Empty);
        }

        private ISelection GetSelectionAfterRemove(ITable table, ISelection selectionToBeRemoved)
        {
            // if nothing is selected, next selection should be empty too
            if (selectionToBeRemoved.Items.Length == 0)
                return new Selection();

            List<int> toBeRemovedIndices = CollectionUtils.Map<object, int>(selectionToBeRemoved.Items,
                delegate(object item) { return table.Items.IndexOf(item); });
            toBeRemovedIndices.Sort();

            // use the first unselected item between the to-be-removed items as the next selection
            foreach (int index in toBeRemovedIndices)
            {
                if (index < table.Items.Count - 1 &&
                    toBeRemovedIndices.Contains(index + 1) == false)
                {
                    return new Selection(table.Items[index + 1]);
                }
            }

            // if there is no item between all the to-be-removed items
            // use the item after the last to-be-removed item as the next selection
            int lastToBeRemovedIndex = toBeRemovedIndices[toBeRemovedIndices.Count - 1];
            if (lastToBeRemovedIndex < table.Items.Count - 1)
                return new Selection(table.Items[lastToBeRemovedIndex + 1]);

            // otherwise, use the item before the first to-be-removed item as the next selection
            if (toBeRemovedIndices[0] > 0)
                return new Selection(table.Items[toBeRemovedIndices[0] - 1]);

            // empty selection
            return new Selection();
	    }

        #endregion
    }
}
