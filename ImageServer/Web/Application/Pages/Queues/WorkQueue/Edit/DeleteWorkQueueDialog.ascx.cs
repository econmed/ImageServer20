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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using SR=ClearCanvas.ImageServer.Web.Application.App_GlobalResources.SR;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// A dialog box that prompts users for confirmation to delete a work queue entry and carries out the deletion if users do so.
    /// </summary>
    /// <remarks>
    /// To use this dialog, caller must indicate the <see cref="WorkQueue"/> entry through the <see cref="WorkQueueItemKey"/> property then
    /// call <see cref="Show"/> to display the dialog. Optionally, caller can register an event listener for <see cref="WorkQueueItemDeleted"/>
    /// which is fired when users confirmed to delete the entry and it was sucessfully deleted.
    /// </remarks>
    public partial class DeleteWorkQueueDialog : UserControl
    {
        private Model.WorkQueue _workQueue;

        #region Events

        #region Delegates

        public delegate void OnHideEventHandler();

        public delegate void OnShowEventHandler();

        /// <summary>
        /// Defines handler for <see cref="WorkQueueItemDeleted"/> event.
        /// </summary>
        /// <param name="item"></param>
        public delegate void WorkQueueItemDeletedListener(Model.WorkQueue item);

        #endregion

        /// <summary>
        /// Fired when the <see cref="WorkQueue"/> object associated with this dialog box is deleted.
        /// </summary>
        public event WorkQueueItemDeletedListener WorkQueueItemDeleted;

        public event OnShowEventHandler OnShow;

        public event OnHideEventHandler OnHide;

        #endregion Events

        #region Public Properties

        public bool IsShown
        {
            get { return ViewState["IsShown"] != null && (bool) ViewState["IsShown"]; }
            set { ViewState["IsShown"] = value; }
        }

        /// <summary>
        /// Sets / Gets the <see cref="ServerEntityKey"/> of the <see cref="WorkQueue"/> item associated with this dialog
        /// </summary>
        public ServerEntityKey WorkQueueItemKey
        {
            get
            {
                if (ViewState["WorkQueueItemKey"] == null) return null;
                else return (ServerEntityKey) ViewState["WorkQueueItemKey"];
            }
            set { ViewState["WorkQueueItemKey"] = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PreDeleteConfirmDialog.Confirmed += PreDeleteConfirmDialog_Confirmed;
            PreDeleteConfirmDialog.Cancel += Hide;
            MessageBox.Cancel += Hide;
            MessageBox.Confirmed += delegate { Hide(); };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack && IsShown)
            {
                DataBind();
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void PreDeleteConfirmDialog_Confirmed(object data)
        {
            Hide();

            var key = data as ServerEntityKey;
            if (key != null)
            {
                var adaptor = new WorkQueueAdaptor();
                Model.WorkQueue item = adaptor.Get(key);
                if (item == null)
                {
                    MessageBox.Message = SR.WorkQueueNotAvailable;
                    MessageBox.MessageType =
                        MessageBox.MessageTypeEnum.ERROR;
                    MessageBox.Show();
                }
                else
                {
                    if (item.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress)
                    {
                        MessageBox.Message = SR.WorkQueueBeingProcessed_CannotDelete;
                        MessageBox.MessageType =
                            MessageBox.MessageTypeEnum.ERROR;
                        MessageBox.Show();
                        return;
                    }

                    try
                    {
                        bool successful;
                        var controller = new WorkQueueController();
                        var items = new List<Model.WorkQueue>();
                        items.Add(item);

                        successful = controller.DeleteWorkQueueItems(items);
                        if (successful)
                        {
                            Platform.Log(LogLevel.Info, "Work Queue item deleted by user : Item Key={0}",
                                         item.GetKey().Key);

                            if (WorkQueueItemDeleted != null)
                                WorkQueueItemDeleted(item);

                            if (OnHide != null) OnHide();
                        }
                        else
                        {
                            Platform.Log(LogLevel.Error,
                                         "PreResetConfirmDialog_Confirmed: Unable to delete work queue item. GUID={0}",
                                         item.GetKey().Key);

                            MessageBox.Message = SR.WorkQueueDeleteFailed;
                            MessageBox.MessageType =
                                MessageBox.MessageTypeEnum.ERROR;
                            MessageBox.Show();
                        }
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error,
                                     "PreResetConfirmDialog_Confirmed: Unable to delete work queue item. GUID={0} : {1}",
                                     item.GetKey().Key, e.StackTrace);

                        MessageBox.Message = String.Format(SR.WorkQueueDeleteFailed_WithException, e.Message);
                        MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                        MessageBox.Show();
                    }
                }
            }
        }

        #endregion Private Methods

        private Model.WorkQueue WorkQueue
        {
            get
            {
                if (_workQueue == null)
                {
                    if (WorkQueueItemKey != null)
                    {
                        var adaptor = new WorkQueueAdaptor();
                        _workQueue = adaptor.Get(WorkQueueItemKey);
                    }
                }

                return _workQueue;
            }
        }

        public ServerEntityKey ServerPartitionKey
        {
            get { return WorkQueue.ServerPartitionKey; }
        }

        #region Public Methods



        public override void DataBind()
        {
            if (WorkQueue != null)
            {
                PreDeleteConfirmDialog.Data = WorkQueueItemKey;
                PreDeleteConfirmDialog.MessageType = MessageBox.MessageTypeEnum.YESNO;
                PreDeleteConfirmDialog.Message = SR.WorkQueueDeleteConfirm;
            }
            else
            {
                MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                MessageBox.Message = SR.WorkQueueNotAvailable;
            }
            base.DataBind();
        }

        /// <summary>
        /// Displays the dialog box for deleting <see cref="WorkQueue"/> entry.
        /// </summary>
        /// <remarks>
        /// The <see cref="WorkQueueItemKey"/> to be deleted must be set prior to calling <see cref="Show"/>.
        /// </remarks>
        public void Show()
        {
            IsShown = true;
            DataBind();
            if (OnShow != null) OnShow();
        }

        protected override void OnPreRender(EventArgs e)
        {
            MessageBox.Close();
            PreDeleteConfirmDialog.Close();
            if (IsShown)
            {
                if (WorkQueue != null)
                {
                    PreDeleteConfirmDialog.Show();
                }
                else
                    MessageBox.Show();
            }


            base.OnPreRender(e);
        }

        /// <summary>
        /// Closes the dialog box
        /// </summary>
        public void Hide()
        {
            IsShown = false;

            if (OnHide != null) OnHide();
        }

        #endregion Public Methods
    }
}