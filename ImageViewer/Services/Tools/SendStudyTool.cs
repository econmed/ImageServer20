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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.Dicom.OffisNetwork;
using System.ServiceModel;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarSendStudy", "SendStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuSendStudy", "SendStudy")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [Tooltip("activate", "TooltipSendStudy")]
	[IconSet("activate", IconScheme.Colour, "Icons.SendStudyToolSmall.png", "Icons.SendStudyToolSmall.png", "Icons.SendStudyToolSmall.png")]
    [ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
    public class SendStudyTool : StudyBrowserTool
    {
        public SendStudyTool()
        {

        }

        private void SendStudy()
        {
			BlockingOperation.Run(SendStudyInternal);
        }

		private void SendStudyInternal()
		{
			if (this.Context.SelectedStudy == null)
				return;

			AENavigatorComponent aeNavigator = new AENavigatorComponent(false, false, false, true);
			SimpleComponentContainer dialogContainer = new SimpleComponentContainer(aeNavigator);

			ApplicationComponentExitCode code =
				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					dialogContainer,
					SR.TitleSendStudy);

			if (code != ApplicationComponentExitCode.Accepted)
				return;

			if (aeNavigator.SelectedServers == null || aeNavigator.SelectedServers.Servers == null || aeNavigator.SelectedServers.Servers.Count == 0)
			{
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageSelectDestination, MessageBoxActions.Ok);
				return;
			}

			if (aeNavigator.SelectedServers.Servers.Count > 1)
			{
				if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmSendToMultipleServers, MessageBoxActions.YesNo) == DialogBoxAction.No)
					return;
			}

			List<string> studyUids = new List<string>();
			foreach (StudyItem item in this.Context.SelectedStudies)
				studyUids.Add(item.StudyInstanceUID);

			DicomServerServiceClient client = new DicomServerServiceClient();

			try
			{
				client.Open();

				foreach (Server destinationAE in aeNavigator.SelectedServers.Servers)
				{
					AEInformation aeInformation = new AEInformation();
					aeInformation.AETitle = destinationAE.AETitle;
					aeInformation.HostName = destinationAE.Host;
					aeInformation.Port = destinationAE.Port;
					
					client.Send(aeInformation, studyUids);
				}

				client.Close();

				LocalDataStoreActivityMonitorComponentManager.ShowSendReceiveActivityComponent(this.Context.DesktopWindow);
			}
			catch (EndpointNotFoundException)
			{
				client.Abort();
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageSendDicomServerServiceNotRunning, MessageBoxActions.Ok);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToSendStudy, this.Context.DesktopWindow);
			}
		}

        protected override void OnSelectedStudyChanged(object sender, EventArgs e)
        {
            // If the results aren't from the local machine, then we don't
            // even care whether a study has been selected or not
            if (!this.Context.SelectedServerGroup.IsLocalDatastore)
                return;

            base.OnSelectedStudyChanged(sender, e);
        }

        protected override void OnSelectedServerChanged(object sender, EventArgs e)
        {
            // If no study is selected then we don't even care whether
            // the last searched server has changed.
            if (this.Context.SelectedStudy == null)
                return;

            if (this.Context.SelectedServerGroup.IsLocalDatastore)
                this.Enabled = true;
            else
                this.Enabled = false;
        }
    }
}
