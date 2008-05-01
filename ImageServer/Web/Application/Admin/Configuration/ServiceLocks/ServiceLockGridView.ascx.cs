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
using ClearCanvas.ImageServer.Web.Common.Data;


namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServiceLocks
{
    //
    //  Used to display the list of services.
    //
    public partial class ServiceLockGridView : UserControl
    {
        #region private members
        private ServiceLockCollection _services;
        private Unit _height;
        FileSystemsConfigurationController _fsController = new FileSystemsConfigurationController();
        #endregion Private members

        #region protected properties

        #endregion protected properties

        #region public properties

        /// <summary>
        /// Retrieve reference to the grid control being used to display the services.
        /// </summary>
        public GridView TheGrid
        {
            get { return GridView; }
        }

        /// <summary>
        /// Gets/Sets the current selected service.
        /// </summary>
        public ServiceLock SelectedServiceLock
        {
            get
            {
                if (ServiceLocks==null || ServiceLocks.Count == 0 || GridView.SelectedIndex < 0)
                    return null;

                // SelectedIndex is for the current page. Must convert to the index of the entire list
                int index = GridView.PageIndex*GridView.PageSize + GridView.SelectedIndex;

                if (index < 0 || index > ServiceLocks.Count - 1)
                    return null;

                return ServiceLocks[index];
            }
            set
            {
                GridView.SelectedIndex = ServiceLocks.IndexOf(value);
                if (OnServiceLockSelectionChanged != null)
                    OnServiceLockSelectionChanged(this, value);
            }
        }

        /// <summary>
        /// Gets/Sets the list of services rendered on the screen.
        /// </summary>
        public ServiceLockCollection ServiceLocks
        {
            get { return _services; }
            set
            {
                _services = value;
            }
        }


        /// <summary>
        /// Gets/Sets the height of service list panel.
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
        #endregion

        #region Events

        /// <summary>
        /// Defines the handler for <seealso cref="OnServiceLockSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedServiceLock"></param>
        public delegate void ServiceLockSelectedEventHandler(object sender, ServiceLock selectedServiceLock);

        /// <summary>
        /// Occurs when the selected service in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected service can change programmatically or by users selecting the service in the list.
        /// </remarks>
        public event ServiceLockSelectedEventHandler OnServiceLockSelectionChanged;

        #endregion // Events

        #region protected methods


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;
        }

        /// <summary>
        /// Updates the grid pager based on the current list.
        /// </summary>
        protected void UpdatePager()
        {
            #region update pager of the gridview if it is used

            if (GridView.BottomPagerRow != null)
            {
                // Show Number of services in the list
                Label lbl = GridView.BottomPagerRow.Cells[0].FindControl("PagerServiceLockCountLabel") as Label;
                if (lbl != null)
                    lbl.Text = string.Format("{0} service(s)", ServiceLocks.Count);

                // Show current page and the number of pages for the list
                lbl = GridView.BottomPagerRow.Cells[0].FindControl("PagerPagingLabel") as Label;
                if (lbl != null)
                    lbl.Text = string.Format("Page {0} of {1}", GridView.PageIndex + 1, GridView.PageCount);

                // Enable/Disable the "Prev" page button
                ImageButton btn = GridView.BottomPagerRow.Cells[0].FindControl("PagerPrevImageButton") as ImageButton;
                if (btn != null)
                {
                    if (ServiceLocks.Count == 0 || GridView.PageIndex == 0)
                    {
                        btn.ImageUrl = "~/images/prev_disabled.gif";
                        btn.Enabled = false;
                    }
                    else
                    {
                        btn.ImageUrl = "~/images/prev.gif";
                        btn.Enabled = true;
                    }

                    btn.Style.Add("cursor", "hand");
                }

                // Enable/Disable the "Next" page button
                btn = GridView.BottomPagerRow.Cells[0].FindControl("PagerNextImageButton") as ImageButton;
                if (btn != null)
                {
                    if (ServiceLocks.Count == 0 || GridView.PageIndex == GridView.PageCount - 1)
                    {
                        btn.ImageUrl = "~/images/next_disabled.gif";
                        btn.Enabled = false;
                    }
                    else
                    {
                        btn.ImageUrl = "~/images/next.gif";
                        btn.Enabled = true;
                    }

                    btn.Style.Add("cursor", "hand");
                }
            }

            #endregion
        }


        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (GridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Add OnClick attribute to each row to make javascript call "Select$###" (where ### is the selected row)
                    // This method when posted back will be handled by the grid
                    e.Row.Attributes["OnClick"] =
                        Page.ClientScript.GetPostBackEventReference(GridView, "Select$" + e.Row.RowIndex);
                    e.Row.Style["cursor"] = "hand";

                    CustomizeEnabledColumn(e.Row);
                    CustomizeLockColumn(e.Row);
                    CustomizeFilesystemColumn(e.Row);
                }
            }
        }

        protected void CustomizeEnabledColumn(GridViewRow row)
        {
            Image img = row.FindControl("EnabledImage") as Image;

            ServiceLock item = row.DataItem as ServiceLock;
            if (img!=null && item != null)
            {
                img.ImageUrl = item.Enabled ? "~/images/checked_small.gif" : "~/images/unchecked_small.gif";
            }
        }
        protected void CustomizeLockColumn(GridViewRow row)
        {
            Image img = row.FindControl("LockedImage") as Image;

            ServiceLock item = row.DataItem as ServiceLock;
            if (img != null && item != null)
            {
                img.ImageUrl = item.Lock ? "~/images/checked_small.gif" : "~/images/unchecked_small.gif";
            }
        }
        protected void CustomizeFilesystemColumn(GridViewRow row)
        {
            Label text = row.FindControl("Filesystem") as Label;

            ServiceLock item = row.DataItem as ServiceLock;
            if (item.FilesystemKey == null)
                text.Text = "N/A";
            else
                text.Text = _fsController.LoadFileSystem(item.FilesystemKey).Description;            
        }

        protected void GridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataBind();
            ServiceLock dev = SelectedServiceLock;
            if (dev != null)
                if (OnServiceLockSelectionChanged != null)
                    OnServiceLockSelectionChanged(this, dev);
        }

        protected void GridView_DataBound(object sender, EventArgs e)
        {
            // reselect the row based on the new order
            if (SelectedServiceLock != null)
            {
                GridView.SelectedIndex = ServiceLocks.RowIndexOf(SelectedServiceLock, GridView);
            }
        }

        protected void GridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void ImageButton_Command(object sender, CommandEventArgs e)
        {
            // get the current page selected
            int intCurIndex = GridView.PageIndex;

            switch (e.CommandArgument.ToString().ToLower())
            {
                case "first":
                    GridView.PageIndex = 0;
                    break;
                case "prev":
                    GridView.PageIndex = intCurIndex - 1;
                    break;
                case "next":
                    GridView.PageIndex = intCurIndex + 1;
                    break;
                case "last":
                    GridView.PageIndex = GridView.PageCount;
                    break;
            }

            DataBind();
        }

        #endregion

        #region public methods

        /// <summary>
        /// Binds the list to the control.
        /// </summary>
        /// <remarks>
        /// This method must be called after setting <seeaslo cref="ServiceLocks"/> to update the grid with the list.
        /// </remarks>
        public override void DataBind()
        {
            if (ServiceLocks!=null)
            {
                GridView.DataSource = ServiceLocks.Values;
            }

            base.DataBind();
        }


        public void Refresh()
        {
            UpdatePanel.Update();
        }

        #endregion // public methods
    }
}
