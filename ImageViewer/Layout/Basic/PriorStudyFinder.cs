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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[ExceptionPolicyFor(typeof(LoadPriorStudiesException))]

	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	public class PriorStudyLoaderExceptionPolicy : IExceptionPolicy
	{
		public PriorStudyLoaderExceptionPolicy()
		{
		}

		#region IExceptionPolicy Members

		public void Handle(System.Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			if (e is LoadPriorStudiesException)
			{
				exceptionHandlingContext.Log(LogLevel.Error, e);
				
				Handle(e as LoadPriorStudiesException, exceptionHandlingContext);
			}
		}

		#endregion

		private static void Handle(LoadPriorStudiesException exception, IExceptionHandlingContext context)
		{
			if (exception.FindFailed)
			{
				context.ShowMessageBox(SR.MessageSearchForPriorsFailed);
			}
			else if (ShouldShowErrorMessage(exception))
			{
				StringBuilder summary = new StringBuilder();

				summary.AppendLine(SR.MessageLoadPriorsErrorPrefix);
				summary.Append(exception.GetExceptionSummary());

				context.ShowMessageBox(summary.ToString());
			}
		}

		private static bool ShouldShowErrorMessage(LoadPriorStudiesException exception)
		{
			if (exception.IncompleteCount > 0)
				return true;

			if (exception.NotFoundCount > 0)
				return true;

			if (exception.UnknownFailureCount > 0)
				return true;

			return false;
		}
	}

	[ExtensionOf(typeof(PriorStudyFinderExtensionPoint))]
	public class PriorStudyFinder : ClearCanvas.ImageViewer.PriorStudyFinder
	{
		private volatile bool _cancel;

		public PriorStudyFinder()
		{
		}

		public override StudyItemList FindPriorStudies()
		{
			_cancel = false;
			StudyItemList results = new StudyItemList();

			DefaultPatientReconciliationStrategy reconciliationStrategy = new DefaultPatientReconciliationStrategy();
			List<string> patientIds = new List<string>();
			foreach (Patient patient in Viewer.StudyTree.Patients)
			{
				if (_cancel)
					break;

				PatientInformation info = new PatientInformation();
				info.PatientId = patient.PatientId;
				PatientInformation reconciled = reconciliationStrategy.ReconcileSearchCriteria(info);
				if (!patientIds.Contains(reconciled.PatientId))
					patientIds.Add(reconciled.PatientId);
			}

			using (StudyRootQueryBridge bridge = new StudyRootQueryBridge(Platform.GetService<IStudyRootQuery>()))
			{
				foreach (string patientId in patientIds)
				{
					StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier();
					identifier.PatientId = patientId;

					IList<StudyRootStudyIdentifier> studies = bridge.StudyQuery(identifier);
					foreach (StudyRootStudyIdentifier study in studies)
					{
						if (_cancel)
							break;

						StudyItem studyItem = ConvertToStudyItem(study);
						if (studyItem != Null)
							results.Add(studyItem);
					}
				}
			}

			return results;
		}

		public override void Cancel()
		{
			_cancel = true;
		}

		private StudyItem ConvertToStudyItem(StudyRootStudyIdentifier study)
		{
			string studyLoaderName;
			ApplicationEntity applicationEntity = null;

			IServerTreeNode node = FindServer(study.RetrieveAeTitle);
			if (node.IsLocalDataStore)
			{
				studyLoaderName = "DICOM_LOCAL";
			}
			else if (node.IsServer)
			{
				Server server = (Server)node;
				if (server.IsStreaming)
					studyLoaderName = "CC_STREAMING";
				else
					studyLoaderName = "DICOM_REMOTE";

				applicationEntity = new ApplicationEntity(server.Host, server.AETitle, server.Name, server.Port,
											server.IsStreaming, server.HeaderServicePort, server.WadoServicePort);
			}
			else // (node == null)
			{
				Platform.Log(LogLevel.Warn,
					String.Format("Unable to find server information '{0}' in order to load study '{1}'",
					study.RetrieveAeTitle, study.StudyInstanceUid));

				return null;
			}

			StudyItem item = new StudyItem(study, applicationEntity, studyLoaderName);
			if (String.IsNullOrEmpty(item.InstanceAvailability))
				item.InstanceAvailability = "ONLINE";

			return item;
		}

		private static IServerTreeNode FindServer(string retrieveAETitle)
		{
			ServerTree serverTree = new ServerTree();
			if (retrieveAETitle == serverTree.RootNode.LocalDataStoreNode.GetClientAETitle())
				return serverTree.RootNode.LocalDataStoreNode;

			List<Server> remoteServers = Configuration.DefaultServers.SelectFrom(serverTree);
			foreach (Server server in remoteServers)
			{
				if (server.AETitle == retrieveAETitle)
					return server;
			}

			return null;
		}
	}
}
