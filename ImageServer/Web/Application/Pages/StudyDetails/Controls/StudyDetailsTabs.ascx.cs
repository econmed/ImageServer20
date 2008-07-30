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
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls
{  
    public partial class StudyDetailsTabs : UserControl
    {

        #region Private Members
        
        private Study _study;
        private ServerPartition _partition;
        
        #endregion Private Members

        #region Public Members

        [ExtenderControlProperty]
        [ClientPropertyName("ViewSeriesButtonClientID")]
        public string ViewSeriesButtonClientID
        {
            get { return ViewSeriesButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("SeriesListClientID")]
        public string SeriesListClientID
        {
            get { return SeriesGridView.SeriesListControl.ClientID; }
        }
       
        [ClientPropertyName("OpenSeriesPageUrl")]
        public string OpenSeriesPageUrl
        {
            get { return Page.ResolveClientUrl(App_GlobalResources.ImageServerPageURLs.SeriesDetailsPage); }
        }

        #endregion Public Members

        /// <summary>
        /// Sets or gets the displayed study
        /// </summary>
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }            
        }

        #region Protected Methods

        protected override void OnPreRender(EventArgs e)
        {
            int[] selectedSeriesIndices = SeriesGridView.SeriesListControl.SelectedIndices;
            ViewSeriesButton.Enabled = selectedSeriesIndices != null && selectedSeriesIndices.Length > 0;

            base.OnPreRender(e);
        }

        #endregion Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override void DataBind()
        {
            // setup the data for the child controls
            if (Study != null)
            {
                StudyDetailsView.Studies.Add(Study);

                SeriesGridView.Partition = Partition;
                SeriesGridView.Study = Study;

                WorkQueueGridView.Study = Study;
                FSQueueGridView.Study = Study;
                StudyStorageView.Study = Study;
            }

            base.DataBind();

        }
    }
}