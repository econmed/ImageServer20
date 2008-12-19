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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies
{
    public class SearchPanelDeleteButtonClickedEventArgs:EventArgs
    {
        private IEnumerable<StudySummary> _selectedStudies;
        public IEnumerable<StudySummary> SelectedStudies
        {
            set { _selectedStudies = value; }
            get { return _selectedStudies; }
        }
    }
    [ClientScriptResource(ComponentType="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel", ResourcePath="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private members
        private ServerPartition _serverPartition;
        private StudyController _controller = new StudyController();
        private EventHandler<SearchPanelDeleteButtonClickedEventArgs> _deleteButtonClickedHandler;
    	#endregion Private members

        #region Events
        public event EventHandler<SearchPanelDeleteButtonClickedEventArgs> DeleteButtonClicked
        {
            add { _deleteButtonClickedHandler += value; }
            remove { _deleteButtonClickedHandler -= value; }
        }
        #endregion

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteStudyButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("OpenButtonClientID")]
        public string OpenButtonClientID
        {
            get { return ViewStudyDetailsButton.ClientID; }
        }

		[ExtenderControlProperty]
		[ClientPropertyName("RestoreButtonClientID")]
		public string RestoreButtonClientID
		{
			get { return RestoreStudyButton.ClientID; }
		}

        [ExtenderControlProperty]
        [ClientPropertyName("SendButtonClientID")]
        public string SendButtonClientID
        {
            get { return MoveStudyButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("StudyListClientID")]
        public string StudyListClientID
        {
            get { return StudyListGridView.StudyListGrid.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("OpenStudyPageUrl")]
        public string OpenStudyPageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.StudyDetailsPage); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("SendStudyPageUrl")]
        public string SendStudyPageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.MoveStudyPage); }
        }

        public ServerPartition ServerPartition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }

        #endregion Public Properties  

        #region Private Methods

        private void SetupChildControls()
        {
            ClearStudyDateButton.OnClientClick = ScriptHelper.ClearDate(StudyDate.ClientID, StudyDateCalendarExtender.ClientID);
            
            GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerStudySingleItem, App_GlobalResources.SR.GridPagerStudyMultipleItems, StudyListGridView.StudyListGrid);
            GridPagerBottom.InitializeGridPager(App_GlobalResources.SR.GridPagerStudySingleItem, App_GlobalResources.SR.GridPagerStudyMultipleItems, StudyListGridView.StudyListGrid);
            GridPagerTop.GetRecordCountMethod = delegate
                              {
                                  return StudyListGridView.ResultCount;
                              };
            GridPagerBottom.GetRecordCountMethod = delegate
                                          {
                                              return StudyListGridView.ResultCount;
                                          };

            RestoreMessageBox.Confirmed += delegate(object data)
                            {
                                if (data is IList<Study>)
                                {
                                    IList<Study> studies = data as IList<Study>;
                                    foreach (Study study in studies)
                                    {
                                        _controller.RestoreStudy(study);
                                    }
                                }
								else if (data is IList<StudySummary>)
								{
									IList<StudySummary> studies = data as IList<StudySummary>;
									foreach (StudySummary study in studies)
									{
										_controller.RestoreStudy(study.TheStudy);
									}
								}
                                else if (data is Study)
                                {
                                    Study study = data as Study;
                                    _controller.RestoreStudy(study);
                                }

                                DataBind();
                                UpdatePanel.Update(); // force refresh
                            };

            StudyListGridView.DataSourceCreated += delegate(StudyDataSource source)
                                        {
                                            source.Partition = ServerPartition;
                                            source.DateFormats = StudyDateCalendarExtender.Format;

                                            if (!String.IsNullOrEmpty(PatientId.Text))
                                                source.PatientId = PatientId.Text;
                                            if (!String.IsNullOrEmpty(PatientName.Text))
                                                source.PatientName = PatientName.Text;
                                            if (!String.IsNullOrEmpty(AccessionNumber.Text))
                                                source.AccessionNumber = AccessionNumber.Text;
                                            if (!String.IsNullOrEmpty(StudyDate.Text))
                                                source.StudyDate = StudyDate.Text;
                                            if (!String.IsNullOrEmpty(StudyDescription.Text))
                                                source.StudyDescription = StudyDescription.Text;

                                            if (ModalityListBox.SelectedIndex > -1)
                                            {
                                                List<string> modalities = new List<string>();
                                                foreach (ListItem item in ModalityListBox.Items)
                                                {
                                                    if (item.Selected)
                                                    {
                                                        modalities.Add(item.Value);
                                                    }
                                                }
                                                source.Modalities = modalities.ToArray();
                                            }
                                        };
        }

        void DeleteStudyConfirmDialog_StudyDeleted(object sender, DeleteStudyConfirmDialogStudyDeletedEventArgs e)
        {
            Refresh();
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientId.Text = string.Empty;
            PatientName.Text = string.Empty;
            AccessionNumber.Text = string.Empty;
            StudyDescription.Text = string.Empty;
            StudyDate.Text = string.Empty;
        }

        public override void DataBind()
        {
            StudyListGridView.Partition = ServerPartition;
            base.DataBind();
            StudyListGridView.DataBind();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetupChildControls();           
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            StudyDate.Text = Request[StudyDate.UniqueID];
            if (!String.IsNullOrEmpty(StudyDate.Text))
                StudyDateCalendarExtender.SelectedDate = DateTime.ParseExact(StudyDate.Text, StudyDateCalendarExtender.Format, null);
            else
                StudyDateCalendarExtender.SelectedDate = null;

			if (StudyListGridView.IsPostBack)
			{
				DataBind();
			}
        }

        public void Refresh()
        {
            StudyListGridView.StudyListGrid.ClearSelections();
            StudyListGridView.StudyListGrid.PageIndex = 0;
            DataBind();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            Refresh();
        }

        
		protected void RestoreStudyButton_Click(object sender, ImageClickEventArgs e)
		{
			IList<StudySummary> studies = StudyListGridView.SelectedStudies;

			if (studies != null && studies.Count > 0)
			{
			    string message = studies.Count > 1 ? string.Format(App_GlobalResources.SR.MultipleStudyRestore):
				                                    string.Format(App_GlobalResources.SR.SingleStudyRestore);

			    RestoreMessageBox.Message = DialogHelper.createConfirmationMessage(message);
                RestoreMessageBox.Message += DialogHelper.createStudyTable(studies);
				
			    RestoreMessageBox.Title = App_GlobalResources.Titles.RestoreStudyConfirmation;
                RestoreMessageBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
				IList<Study> studyList = new List<Study>();
				foreach (StudySummary summary in studies)
					studyList.Add(summary.TheStudy);
				RestoreMessageBox.Data = studyList;
				RestoreMessageBox.Show();
			}
		}

        #endregion Protected Methods

        protected void DeleteStudyButton_Click(object sender, ImageClickEventArgs e)
        {
            SearchPanelDeleteButtonClickedEventArgs args = new SearchPanelDeleteButtonClickedEventArgs();
            args.SelectedStudies = StudyListGridView.SelectedStudies;
            EventsHelper.Fire(_deleteButtonClickedHandler, this, args);
        }
    }
}