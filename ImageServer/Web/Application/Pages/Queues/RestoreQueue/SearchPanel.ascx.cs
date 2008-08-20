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
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue
{
    [ClientScriptResource(ComponentType="ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel", ResourcePath="ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private members

        private RestoreQueueController _controller = new RestoreQueueController();

    	#endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteItemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ItemListClientID")]
        public string ItemListClientID
        {
            get { return RestoreQueueItemList.RestoreQueueGrid.ClientID; }
        }

        #endregion Public Properties  

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientId.Text = string.Empty;
            PatientName.Text = string.Empty;
            ScheduleDate.Text = string.Empty;
            StatusFilter.SelectedIndex = 0;
        }

        public override void DataBind()
        {
            RestoreQueueItemList.Partition = ((Default)Page).ServerPartition;
            base.DataBind();
            RestoreQueueItemList.DataBind();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ScheduleDateCalendarExtender.Format = DateTimeFormatter.DefaultDateFormat;

            ClearScheduleDateButton.OnClientClick = "document.getElementById('" + ScheduleDate.ClientID + "').value=''; return false;";
            
            // setup child controls
            GridPagerBottom.Target = RestoreQueueItemList.RestoreQueueGrid;

            GridPagerTop.ItemName = App_GlobalResources.Labels.GridPagerRestoreQueueSingleItem;
            GridPagerTop.PuralItemName = App_GlobalResources.Labels.GridPagerRestoreQueueMultipleItems;
            GridPagerTop.Target = RestoreQueueItemList.RestoreQueueGrid;
            GridPagerTop.GetRecordCountMethod = delegate
                              {
								  return RestoreQueueItemList.ResultCount;
                              };

            GridPagerBottom.ItemName = App_GlobalResources.Labels.GridPagerRestoreQueueSingleItem;
            GridPagerBottom.PuralItemName = App_GlobalResources.Labels.GridPagerRestoreQueueMultipleItems;
            GridPagerBottom.Target = RestoreQueueItemList.RestoreQueueGrid;
            GridPagerBottom.GetRecordCountMethod = delegate
                              {
                                  return RestoreQueueItemList.ResultCount;
                              };


            MessageBox.Confirmed += delegate(object data)
                            {
                                if (data is IList<Model.RestoreQueue>)
                                {
                                    IList<Model.RestoreQueue> items = data as IList<Model.RestoreQueue>;
                                    foreach (Model.RestoreQueue item in items)
                                    {
                                        _controller.DeleteRestoreQueueItem(item);
                                    }
                                }
                                else if (data is Model.RestoreQueue)
                                {
                                    Model.RestoreQueue item = data as Model.RestoreQueue;
                                    _controller.DeleteRestoreQueueItem(item);
                                }

                                DataBind();
                                UpdatePanel.Update(); // force refresh

                            };

			RestoreQueueItemList.DataSourceCreated += delegate(RestoreQueueDataSource source)
										{
											source.Partition = ((Default)Page).ServerPartition;
                                            source.DateFormats = ScheduleDateCalendarExtender.Format;

											if (!String.IsNullOrEmpty(PatientId.Text))
												source.PatientId = PatientId.Text;
											if (!String.IsNullOrEmpty(PatientName.Text))
												source.PatientName = PatientName.Text;
											if (!String.IsNullOrEmpty(ScheduleDate.Text))
												source.ScheduledDate = ScheduleDate.Text;
										};
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ScheduleDate.Text = Request[ScheduleDate.UniqueID];
            if (!String.IsNullOrEmpty(ScheduleDate.Text))
                ScheduleDateCalendarExtender.SelectedDate = DateTime.ParseExact(ScheduleDate.Text, ScheduleDateCalendarExtender.Format, null);
            else
                ScheduleDateCalendarExtender.SelectedDate = null;

            IList<RestoreQueueStatusEnum> statusItems = RestoreQueueStatusEnum.GetAll();

            int prevSelectedIndex = StatusFilter.SelectedIndex;
            StatusFilter.Items.Clear();
            StatusFilter.Items.Add(new ListItem("All", "All"));
            foreach (RestoreQueueStatusEnum s in statusItems)
                StatusFilter.Items.Add(new ListItem(s.Description, s.Lookup));
            StatusFilter.SelectedIndex = prevSelectedIndex;

			if (RestoreQueueItemList.IsPostBack)
			{
				DataBind();
			} 
        }

        protected override void OnPreRender(EventArgs e)
        {

			UpdateUI();
			base.OnPreRender(e);
        }

        protected void UpdateUI()
        {
            UpdateToolbarButtonState();
        }
        
        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            RestoreQueueItemList.RestoreQueueGrid.ClearSelections();
        	RestoreQueueItemList.RestoreQueueGrid.PageIndex = 0;
			DataBind();
        }

        protected void DeleteItemButton_Click(object sender, EventArgs e)
        {
            IList<Model.RestoreQueue> items = RestoreQueueItemList.SelectedItems;

            if (items != null && items.Count>0)
            {
                if (items.Count > 1) MessageBox.Message = string.Format(App_GlobalResources.SR.MultipleRestoreQueueDelete);
                else MessageBox.Message = string.Format(App_GlobalResources.SR.SingleRestoreQueueDelete);

                MessageBox.Message += "<table>";
                foreach (Model.RestoreQueue item in items)
                {
                    String text = "";
                    //String.Format("<tr align='left'><td>Patient:{0}&nbsp;&nbsp;</td><td>Accession:{1}&nbsp;&nbsp;</td><td>Description:{2}</td></tr>", 
                                    //item.PatientsName, item.AccessionNumber, item.StudyDescription);
                    MessageBox.Message += text;
                }
                MessageBox.Message += "</table>";

                MessageBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
                MessageBox.Data = items;
                MessageBox.Show();
            }
        }

        protected void UpdateToolbarButtonState()
        {
            IList<Model.RestoreQueue> items = RestoreQueueItemList.SelectedItems;
            if (items != null)
            {
				DeleteItemButton.Enabled = true;
            }
            else
            {
                DeleteItemButton.Enabled = false;
            }
        }

        #endregion Protected Methods
    }
}