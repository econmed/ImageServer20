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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using System.ServiceModel;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarRetrieveStudy", "RetrieveStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuRetrieveStudy", "RetrieveStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipRetrieveStudy")]
	[IconSet("activate", IconScheme.Colour, "Icons.RetrieveStudyToolSmall.png", "Icons.RetrieveStudyToolSmall.png", "Icons.RetrieveStudyToolSmall.png")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class RetrieveStudyTool : StudyBrowserTool
	{
		public RetrieveStudyTool()
		{

		}

		public override void Initialize()
		{
			SetDoubleClickHandler();

			base.Initialize();
		}

		private void RetrieveStudy()
		{
			if (!Enabled || this.Context.SelectedServerGroup.IsLocalDatastore || this.Context.SelectedStudy == null)
                return;

			Dictionary<ApplicationEntity, List<StudyInformation>> retrieveInformation = new Dictionary<ApplicationEntity, List<StudyInformation>>();
			foreach (StudyItem item in this.Context.SelectedStudies)
			{
				if (!retrieveInformation.ContainsKey(item.Server))
					retrieveInformation[item.Server] = new List<StudyInformation>();

				StudyInformation studyInformation = new StudyInformation();
				studyInformation.PatientId = item.PatientId;
				studyInformation.PatientsName = item.PatientsName;
				DateTime studyDate;
				DateParser.Parse(item.StudyDate, out studyDate);
				studyInformation.StudyDate = studyDate;
				studyInformation.StudyDescription = item.StudyDescription;
				studyInformation.StudyInstanceUid = item.StudyInstanceUID;

				retrieveInformation[item.Server].Add(studyInformation);
			}

			DicomServerServiceClient client = new DicomServerServiceClient();

			try
			{
				client.Open();

				foreach (KeyValuePair<ApplicationEntity, List<StudyInformation>> kvp in retrieveInformation)
				{
					AEInformation aeInformation = new AEInformation();
					aeInformation.AETitle = kvp.Key.AETitle;
					aeInformation.HostName = kvp.Key.Host;
					aeInformation.Port = kvp.Key.Port;

					client.RetrieveStudies(aeInformation, kvp.Value);
				}

				client.Close();

				LocalDataStoreActivityMonitorComponentManager.ShowSendReceiveActivityComponent(this.Context.DesktopWindow);
			}
			catch (EndpointNotFoundException)
			{
				client.Abort();
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageRetrieveDicomServerServiceNotRunning, MessageBoxActions.Ok);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToRetrieveStudy, this.Context.DesktopWindow);
			}
		}

		private void SetDoubleClickHandler()
		{
			if (!this.Context.SelectedServerGroup.IsLocalDatastore &&
				!this.Context.SelectedServerGroup.IsOnlyStreamingServers())
				this.Context.DefaultActionHandler = RetrieveStudy;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}
		
		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
			SetDoubleClickHandler();
        }

		private void UpdateEnabled()
		{
			this.Enabled = (this.Context.SelectedStudy != null &&
			                !this.Context.SelectedServerGroup.IsLocalDatastore &&
			                LocalDataStoreActivityMonitor.Instance.IsConnected);
		}
	}
}
