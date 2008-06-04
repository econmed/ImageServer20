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

namespace ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue.Edit
{
    /// <summary>
    /// The view control to display detail information of a <see cref="WorkQueue"/> item of type 'Auto-Route' inside the <see cref="WorkQueueItemDetailsPanel"/>
    /// </summary>
    public partial class AutoRouteWorkQueueDetailsView : WorkQueueDetailsViewBase
    {
        #region Private members

        #endregion Private members

        #region Public Properties

        /// <summary>
        /// Sets or gets the width of work queue details view panel
        /// </summary>
        public override Unit Width
        {
            get { return base.Width; }
            set { 
                base.Width = value;
                AutoRouteDetailsView.Width = value;
            }
        }
        #endregion Public Properties

        #region Protected Methods

        protected void GeneralInfoDetailsView_DataBound(object sender, EventArgs e)
        {
            WorkQueueDetails item = GeneralInfoDetailsView.DataItem as WorkQueueDetails;
            if (item != null)
            {
                Label statusLabel = GeneralInfoDetailsView.FindControl("Status") as Label;
                if (statusLabel != null)
                {
                    statusLabel.Text = item.Status.Description;
                }

                Label priorityLabel = GeneralInfoDetailsView.FindControl("Priority") as Label;
                if (priorityLabel != null)
                {
                    priorityLabel.Text = item.Priority.Description;
                }
            }


        }

        protected void AutoRouteDetailsView_DataBound(object sender, EventArgs e)
        {
            WorkQueueDetails item = AutoRouteDetailsView.DataItem as WorkQueueDetails;
            if (item != null)
            {
                Label typeLabel = AutoRouteDetailsView.FindControl("Type") as Label;
                if (typeLabel != null)
                {
                    typeLabel.Text = item.Type.Description;
                }

            }
        }
        #endregion Protected Methods

        #region Public Methods


        public override void DataBind()
        {
            if (WorkQueue != null)
            {
                List<WorkQueueDetails> detailsList = new List<WorkQueueDetails>();
                detailsList.Add(WorkQueueDetailsAssembler.CreateWorkQueueDetail(WorkQueue));
                GeneralInfoDetailsView.DataSource = detailsList;
                AutoRouteDetailsView.DataSource = detailsList;
            }
            else
            {
                GeneralInfoDetailsView.DataSource = null;
                AutoRouteDetailsView.DataSource = null;
            }

            base.DataBind();
        }


        #endregion Public Methods


    }
}