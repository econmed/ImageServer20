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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions
{
    /// <summary>
    /// Partition list view panel.
    /// </summary>
    public partial class ServerPartitionGridPanel : UserControl
    {
        #region Private Members

        /// <summary>
        /// list of partitions rendered on the screen.
        /// </summary>
        private IList<ServerPartition> _partitions;
        private Unit _height;
        #endregion private Members

        #region Public Properties

        /// <summary>
        /// Sets/Gets the list of partitions rendered on the screen.
        /// </summary>
        public IList<ServerPartition> Partitions
        {
            get { return _partitions; }
            set
            {
                _partitions = value;
                PartitionGridView.DataSource = _partitions;
            }
        }

        /// <summary>
        /// Retrieves a reference to the embedded grid.
        /// </summary>
        public GridView TheGrid
        {
            get { return PartitionGridView; }
        }

        /// <summary>
        /// Retrieve the current selected partition.
        /// </summary>
        public ServerPartition SelectedPartition
        {
            get
            {
                int index = TheGrid.PageIndex*TheGrid.PageSize + TheGrid.SelectedIndex;

                if (index < 0 || index >= Partitions.Count)
                    return null;

                return Partitions[index];
            }
        }

        /// <summary>
        /// Gets/Sets the height of server partition list panel.
        /// </summary>
        public Unit Height
        {
            get
            {
                if (ContainerTable != null)
                    return ContainerTable.Height;
                else
                    return _height;
            }
            set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
            }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Height != Unit.Empty)
                ContainerTable.Height = _height;


            TheGrid.PagerSettings.Visible = false; // hide the paging control... we use a custom one outside this panel
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }

        protected void PartitionGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (PartitionGridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Add OnClick attribute to each row to make javascript call "Select$###" (where ### is the selected row)
                    // This method when posted back will be handled by the grid
                    e.Row.Attributes["OnClick"] =
                        Page.ClientScript.GetPostBackEventReference(PartitionGridView, "Select$" + e.Row.RowIndex);
                    e.Row.Style["cursor"] = "hand";

                    // For some reason, double-click won't work if single-click is used
                    // e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackEventReference(GridView1, "Edit$" + e.Row.RowIndex);

                    CustomizeActiveColumn(e);
                    CustomizeAcceptAnyDeviceColumn(e);
                }
            }
        }

        protected void CustomizeActiveColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image) e.Row.FindControl("ActiveImage"));

            if (img != null)
            {
                bool active = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                if (active)
                    img.ImageUrl = "~/images/checked_small.gif";
                else
                {
                    img.ImageUrl = "~/images/unchecked_small.gif";
                }
            }
        }

        protected void CustomizeAcceptAnyDeviceColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image) e.Row.FindControl("AcceptAnyDeviceImage"));

            if (img != null)
            {
                bool acceptAnyDevice = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "AcceptAnyDevice"));
                if (acceptAnyDevice)
                    img.ImageUrl = "~/images/checked_small.gif";
                else
                {
                    img.ImageUrl = "~/images/unchecked_small.gif";
                }
            }
        }

        #endregion Protected methods

        #region Public methods

        public void UpdateUI()
        {
            DataBind();

            //  UpdatePanel1.Update(); // force refresh
        }

        #endregion Public methods
    }
}