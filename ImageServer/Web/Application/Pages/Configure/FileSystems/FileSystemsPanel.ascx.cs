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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Configure.FileSystems
{
    /// <summary>
    /// Panel to display list of FileSystems for a particular server partition.
    /// </summary>
    public partial class FileSystemsPanel : UserControl
    {
        #region Private members

        // the controller used for interaction with the database.
        private FileSystemsConfigurationController _theController;
        // the filesystems whose information will be displayed in this panel
        private IList<Filesystem> _filesystems;
        // list of filesystem tiers users can filter on
        private IList<FilesystemTierEnum> _tiers;

        #endregion Private members

        #region Public Properties

        /// <summary>
        /// Sets/Gets the filesystems whose information are displayed in this panel.
        /// </summary>
        public IList<Filesystem> FileSystems
        {
            get { return _filesystems; }
            set { _filesystems = value; }
        }

        /// <summary>
        /// Sets or gets the list of filesystems users can filter.
        /// </summary>
        public IList<FilesystemTierEnum> Tiers
        {
            get { return _tiers; }
            set { _tiers = value; }
        }

        private Default _enclosingPage;

        public Default EnclosingPage
        {
            get { return _enclosingPage; }
            set { _enclosingPage = value; }
        }

        #endregion

        #region protected methods

        /// <summary>
        /// Set up event handlers for the child controls.
        /// </summary>
        protected void SetUpEventHandlers()
        {
            GridPagerTop.GetRecordCountMethod = delegate { return FileSystemsGridView1.FileSystems.Count; };
            GridPagerBottom.GetRecordCountMethod = delegate { return FileSystemsGridView1.FileSystems.Count; };
        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdateUI();
            base.OnPreRender(e);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new FileSystemsConfigurationController();

            // setup child controls
            GridPagerTop.ItemName = App_GlobalResources.SR.GridPagerFileSystemSingleItem;
            GridPagerTop.PuralItemName = App_GlobalResources.SR.GridPagerFileSystemMultipleItems;
            GridPagerTop.Target = FileSystemsGridView1.TheGrid;

            GridPagerBottom.ItemName = App_GlobalResources.SR.GridPagerFileSystemSingleItem;
            GridPagerBottom.PuralItemName = App_GlobalResources.SR.GridPagerFileSystemMultipleItems;
            GridPagerBottom.Target = FileSystemsGridView1.TheGrid;

            Tiers = _theController.GetFileSystemTiers();

            // setup event handler for child controls
            SetUpEventHandlers();

            int prevSelectIndex = TiersDropDownList.SelectedIndex;
            if (TiersDropDownList.Items.Count == 0)
            {
                TiersDropDownList.Items.Add(new ListItem(App_GlobalResources.SR.All));
                foreach (FilesystemTierEnum tier in Tiers)
                {
                    TiersDropDownList.Items.Add(new ListItem(tier.Description, tier.Lookup));
                }
            }
            TiersDropDownList.SelectedIndex = prevSelectIndex;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // This make sure we have the list to work with. 
            // the list may be out-dated if the add/update event is fired later
            // In those cases, the list must be refreshed again.
            LoadFileSystems();
        }

        #endregion Protected methods

        /// <summary>
        /// Load the FileSystems for the partition based on the filters specified in the filter panel.
        /// </summary>
        /// <remarks>
        /// This method only reloads and binds the list bind to the internal grid. <seealso cref="UpdateUI()"/> should be called
        /// to explicit update the list in the grid. 
        /// <para>
        /// This is intentionally so that the list can be reloaded so that it is available to other controls during postback.  In
        /// some cases we may not want to refresh the list if there's no change. Calling <seealso cref="UpdateUI()"/> will
        /// give performance hit as the data will be transfered back to the browser.
        ///  
        /// </para>
        /// </remarks>
        public void LoadFileSystems()
        {
            FilesystemSelectCriteria criteria = new FilesystemSelectCriteria();


            if (String.IsNullOrEmpty(DescriptionFilter.Text) == false)
            {
                string key = DescriptionFilter.Text.Replace("*", "%") + "%";
                criteria.Description.Like(key);
            }

            if (TiersDropDownList.SelectedIndex >= 1) /* 0 = "All" */
                criteria.FilesystemTierEnum.EqualTo(Tiers[TiersDropDownList.SelectedIndex - 1]);

            FileSystemsGridView1.FileSystems = _theController.GetFileSystems(criteria);
            FileSystemsGridView1.DataBind();
        }

        /// <summary>
        /// Updates the FileSystem list window in the panel.
        /// </summary>
        /// <remarks>
        /// This method should only be called when necessary as the information in the list window needs to be transmitted back to the client.
        /// If the list is not changed, call <seealso cref="LoadFileSystems()"/> instead.
        /// </remarks>
        public void UpdateUI()
        {
            LoadFileSystems();

            Filesystem dev = FileSystemsGridView1.SelectedFileSystem;
            if (dev == null)
            {
                // no FileSystem being selected
                EditFileSystemButton.Enabled = false;
            }
            else
            {
                EditFileSystemButton.Enabled = true;
            }

            // UpdatePanel UpdateMode must be set to "conditional"
            // Calling UpdatePanel.Update() will force the client to refresh the screen
            UpdatePanel.Update();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            //UpdateUI();
        }


        protected void AddFileSystemButton_Click(object sender, ImageClickEventArgs e)
        {
            EnclosingPage.OnAddFileSystem();
        }

        protected void EditFileSystemButton_Click(object sender, ImageClickEventArgs e)
        {
            // Call the edit filesystem delegate 
            Filesystem fs = FileSystemsGridView1.SelectedFileSystem;
            if (fs != null)
            {
                EnclosingPage.OnEditFileSystem(_theController, fs);
            }
        }

        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            //UpdateUI();
        }
    }
}