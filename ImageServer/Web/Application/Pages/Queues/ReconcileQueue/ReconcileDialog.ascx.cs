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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.ReconcileQueue
{
    //
    // Dialog for adding a new device or editting an existing device.
    //
    public partial class ReconcileDialog : UserControl
    {
        #region private variables

        // The server partitions that the new device can be associated with
        // This list will be determined by the user level permission.
        private IList<ServerPartition> _partitions = new List<ServerPartition>();

        private Model.ReconcileQueue _item = null;
        private ReconcileDetails _details = null;

        #endregion

        #region public members

        /// <summary>
        /// Sets the list of partitions users allowed to pick.
        /// </summary>
        public IList<ServerPartition> Partitions
        {
            set { _partitions = value; }

            get { return _partitions; }
        }

        /// <summary>
        /// Sets or gets the Reconcile Item Value
        /// </summary>
        public Model.ReconcileQueue ReconcileQueueItem
        {
            get { return _item; }
            set
            {
                _item = value;
                ViewState[ClientID + "_ReconcileQueueItem"] = _item.GetKey();
            }
        }

        /// <summary>
        /// Sets or gets the Reconcile Item Value
        /// </summary>
        public ReconcileDetails ReconcileDetails
        {
            get { return _details; }
            set { 
                  _details = value;

                }
        }


        #endregion // public members

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        /// <param name="device">The device being added.</param>
        public delegate void OnOKClickedEventHandler(Device device);

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events

        #region Public delegates

        #endregion // public delegates

        #region Protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void NewStudyButton_Click(object sender, EventArgs e)
        {
            ServerEntityKey itemKey = ViewState[ClientID + "_ReconcileQueueItem"] as ServerEntityKey;
            ReconcileQueueController controller = new ReconcileQueueController();
            controller.CreateNewStudy(itemKey);

            Close();
        }

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DiscardButton_Click(object sender, EventArgs e)
        {
            ServerEntityKey itemKey = ViewState[ClientID + "_ReconcileQueueItem"] as ServerEntityKey;
            ReconcileQueueController controller = new ReconcileQueueController();
            controller.Discard(itemKey);
            
            Close();
        }

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        private void DisplayReconcileSummary()
        {
            StudyInstanceUIDLabel.Text = ReconcileDetails.StudyInstanceUID;
            ExistingNameLabel.Text = ReconcileDetails.ExistingPatient.Name;
            ConflictingNameLabel.Text = ReconcileDetails.ConflictingPatient.Name;

            ExistingPatientSeriesGridView.DataSource = ReconcileDetails.ExistingPatient.Series;
            ExistingPatientSeriesGridView.DataBind();

            ConflictingPatientSeriesGridView.DataSource = ReconcileDetails.ConflictingPatient.Series;
            ConflictingPatientSeriesGridView.DataBind();
        }

        #endregion Protected methods

        #region Public methods

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show()
        {
            DisplayReconcileSummary();
            ReconcileItemModalDialog.Title = App_GlobalResources.Titles.ReconcileStudyDialog;
            ReconcileItemModalDialog.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            ReconcileItemModalDialog.Hide();
        }

        #endregion Public methods
    }
}
