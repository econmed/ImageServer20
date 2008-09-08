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
using System.Web.UI.WebControls;
using System.Collections.Generic;

using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies
{
    //
    //  Used to display the list of studies.
    //
    public partial class StudyListGridView : System.Web.UI.UserControl
    {
		#region Delegates
		public delegate void StudyDataSourceCreated(StudyDataSource theSource);
		public event StudyDataSourceCreated DataSourceCreated;
		#endregion

        #region Private members
        // list of studies to display
        private IList<StudySummary> _studies;
        private ServerPartition _partition;
        private Unit _height;
    	private StudyDataSource _dataSource;
        #endregion Private members

        #region Public properties

		public int ResultCount
		{
			get
			{
				if (_dataSource == null)
				{
					_dataSource = new StudyDataSource();

					_dataSource.StudyFoundSet += delegate(IList<StudySummary> newlist)
											{
												Studies = newlist;
											};
					if (DataSourceCreated != null)
						DataSourceCreated(_dataSource);
					_dataSource.SelectCount();
				}
				if (_dataSource.ResultCount == 0)
				{
					if (DataSourceCreated != null)
						DataSourceCreated(_dataSource);

					_dataSource.SelectCount();
				}
				return _dataSource.ResultCount;
			}
		}

        /// <summary>
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView StudyListGrid
        {
            get { return this.StudyListControl; }
        }


        public ServerPartition Partition
        {
            set { _partition = value; }
            get { return _partition; }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public IList<Study> SelectedStudies
        {
            get
            {
                if (Studies==null || Studies.Count == 0)
                    return null;

                int[] rows = StudyListControl.SelectedIndices;
                if (rows == null || rows.Length == 0)
                    return null;

				IList<Study> studies = new List<Study>();
                for(int i=0; i<rows.Length; i++)
                {
					if (rows[i] < Studies.Count)
						studies.Add(Studies[rows[i]].TheStudy);
                }

                return studies;
            }
        }

        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
        /// </summary>
        public IList<StudySummary> Studies
        {
            get
            {
                return _studies;
            }
            set
            {
                _studies = value;
            }
        }

        /// <summary>
        /// Gets/Sets the height of the study list panel
        /// </summary>
        public Unit Height
        {
            get {
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
        /// Defines the handler for <seealso cref="OnStudySelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedStudies"></param>
        public delegate void StudySelectedEventHandler(object sender, IList<Model.Study> selectedStudies);

        /// <summary>
        /// Occurs when the selected device in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected device can change programmatically or by users selecting the device in the list.
        /// </remarks>
        public event StudySelectedEventHandler OnStudySelectionChanged;

        #endregion // Events
        
        #region protected methods


        protected void Page_Load(object sender, EventArgs e)
        {
			StudyListControl.DataBind();
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            // The embeded grid control will show pager control if "allow paging" is set to true
            // We want to use our own pager control instead so let's hide it.
            StudyListControl.SelectedIndexChanged += new EventHandler(StudyListControl_SelectedIndexChanged);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

			if (Studies == null)
				return;

            foreach (GridViewRow row in StudyListControl.Rows)
            {
                if (row.RowType==DataControlRowType.DataRow)
                {
					StudySummary study = Studies[row.RowIndex];

					if (study != null)
					{
						row.Attributes.Add("instanceuid", study.TheStudy.StudyInstanceUid);
						row.Attributes.Add("serverae", study.ThePartition.AeTitle);
						if (study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.DeleteScheduled)
						    || study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.PurgeScheduled)
							|| study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.EditScheduled)
							|| study.QueueStudyStateEnum.Equals(QueueStudyStateEnum.ProcessingScheduled))
							row.Attributes.Add("deleted", "true");
						if (study.StudyStatusEnum.Equals(StudyStatusEnum.Nearline))
							row.Attributes.Add("nearline", "true");
					}
                }
            }
        }


        protected void StudyListControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            IList<Study> studies = SelectedStudies;
            if (studies != null)
                if (OnStudySelectionChanged != null)
                    OnStudySelectionChanged(this, studies);            
        }

        protected void StudyListControl_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void StudyListControl_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            StudyListControl.PageIndex = e.NewPageIndex;
            DataBind();
        }

		protected void DisposeStudyDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
		{
			e.Cancel = true;
		}

		protected void GetStudyDataSource(object sender, ObjectDataSourceEventArgs e)
		{
			if (_dataSource == null)
			{
				_dataSource = new StudyDataSource();

				_dataSource.StudyFoundSet += delegate(IList<StudySummary> newlist)
										{
											Studies = newlist;
										};
			}

			e.ObjectInstance = _dataSource;

			if (DataSourceCreated != null)
				DataSourceCreated(_dataSource);

		}
        #endregion
    }

}