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
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.App_GlobalResources;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly:
    WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.js",
        "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    [ClientScriptResource(
        ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel",
        ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private members

        #endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("ReconcileButtonClientID")]
        public string ReconcileButtonClientID
        {
            get { return ReconcileButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ItemListClientID")]
        public string ItemListClientID
        {
            get { return StudyIntegrityQueueItemList.StudyIntegrityQueueGrid.ClientID; }
        }

        /// <summary>
        /// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition ServerPartition { get; set; }

        #endregion Public Properties  

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientName.Text = string.Empty;
            PatientId.Text = string.Empty;
            AccessionNumber.Text = string.Empty;
            FromDate.Text = string.Empty;
            ToDate.Text = string.Empty;
        }

        public void UpdateUI()
        {
            SearchUpdatePanel.Update();
            StudyIntegrityQueueItemList.RefreshCurrentPage();
        }

        #endregion Public Methods

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !Page.IsAsync)
            {
                string patientID = Request["PatientID"];
                string patientName = Request["PatientName"];
                string partitionKey = Request["PartitionKey"];
                string reason = Request["Reason"];
                string databind = Request["Databind"];

                if (patientID != null && patientName != null && partitionKey != null)
                {
                    var controller = new ServerPartitionConfigController();
                    ServerPartition = controller.GetPartition(new ServerEntityKey("ServerPartition", partitionKey));

                    PatientId.Text = patientID;
                    PatientName.Text = patientName;

                    StudyIntegrityQueueItemList.SetDataSource();
                    StudyIntegrityQueueItemList.Refresh();
                }
                if (reason != null)
                {
                    ReasonListBox.Items.FindByValue(reason).Selected = true;

                    StudyIntegrityQueueItemList.SetDataSource();
                    StudyIntegrityQueueItemList.Refresh();
                }
                if (databind != null)
                {
                    StudyIntegrityQueueItemList.SetDataSource();
                    StudyIntegrityQueueItemList.Refresh();
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearFromDateButton.OnClientClick = ScriptHelper.ClearDate(FromDate.ClientID,
                                                                           FromDateCalendarExtender.ClientID);
            ClearToDateButton.OnClientClick = ScriptHelper.ClearDate(ToDate.ClientID,
                                                                           ToDateCalendarExtender.ClientID);
            ToDate.Attributes["OnChange"] = ScriptHelper.CheckDateRange(FromDate.ClientID, ToDate.ClientID, ToDate.ClientID, ToDateCalendarExtender.ClientID, "To Date must be greater than From Date");
            FromDate.Attributes["OnChange"] = ScriptHelper.CheckDateRange(FromDate.ClientID, ToDate.ClientID, FromDate.ClientID, FromDateCalendarExtender.ClientID, "From Date must be less than To Date");

            GridPagerTop.InitializeGridPager(Labels.GridPagerQueueSingleItem,
                                             Labels.GridPagerQueueMultipleItems,
                                             StudyIntegrityQueueItemList.StudyIntegrityQueueGrid,
                                             () => StudyIntegrityQueueItemList.ResultCount,
                                             ImageServerConstants.GridViewPagerPosition.Top);
            StudyIntegrityQueueItemList.Pager = GridPagerTop;

            StudyIntegrityQueueItemList.DataSourceCreated += delegate(StudyIntegrityQueueDataSource source)
                                                                 {
                                                                     source.Partition = ServerPartition;

                                                                     if (!String.IsNullOrEmpty(PatientName.Text))
                                                                         source.PatientName = "*" + PatientName.Text +
                                                                                              "*";
                                                                     if (!String.IsNullOrEmpty(PatientId.Text))
                                                                         source.PatientId = "*" + PatientId.Text + "*";
                                                                     if (!String.IsNullOrEmpty(AccessionNumber.Text))
                                                                         source.AccessionNumber = "*" +
                                                                                                  AccessionNumber.Text +
                                                                                                  "*";
                                                                     if (!String.IsNullOrEmpty(FromDate.Text))
                                                                         source.FromInsertTime = FromDate.Text;

                                                                     if (!String.IsNullOrEmpty(ToDate.Text))
                                                                         source.ToInsertTime = ToDate.Text;

                                                                     if (ReasonListBox.SelectedIndex > -1)
                                                                     {
                                                                         var reasonEnums =
                                                                             new List<StudyIntegrityReasonEnum>();
                                                                         foreach (ListItem item in ReasonListBox.Items)
                                                                         {
                                                                             if (item.Selected)
                                                                             {
                                                                                 reasonEnums.Add(
                                                                                     StudyIntegrityReasonEnum.GetEnum(
                                                                                         item.Value));
                                                                             }
                                                                         }

                                                                         source.ReasonEnum = reasonEnums;
                                                                     }
                                                                 };

            ReconcileButton.Roles =
                AuthorityTokens.StudyIntegrityQueue.Reconcile;

            List<StudyIntegrityReasonEnum> reasons = StudyIntegrityReasonEnum.GetAll();
            foreach (StudyIntegrityReasonEnum reason in reasons)
            {
                ReasonListBox.Items.Add(new ListItem(reason.Description, reason.Lookup));
            }
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            StudyIntegrityQueueItemList.Refresh();
        }

        protected void ReconcileButton_Click(object sender, EventArgs e)
        {
            ReconcileDetails details =
                ReconcileDetailsAssembler.CreateReconcileDetails(StudyIntegrityQueueItemList.SelectedItems[0]);

            ((Default) Page).OnReconcileItem(details);
        }

        #endregion Protected Methods
    }
}