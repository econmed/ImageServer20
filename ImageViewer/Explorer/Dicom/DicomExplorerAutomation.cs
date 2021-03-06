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
using System.ServiceModel;
using System.ServiceModel.Description;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.DesktopServices;
using ClearCanvas.ImageViewer.Services.Automation;
using ClearCanvas.ImageViewer.Services.ServerTree;
using System.Threading;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	#region Hosting

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class DicomExplorerAutomationServiceHostTool : DesktopServiceHostTool
	{
		public DicomExplorerAutomationServiceHostTool()
		{
		}

		protected override ServiceHost CreateServiceHost()
		{
			ServiceHost host = new ServiceHost(typeof(DicomExplorerAutomation));
			foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
				endpoint.Binding.Namespace = AutomationNamespace.Value;

			return host;
		}
	}

	#endregion

	//Note: should the need arise, we could later allow the different explorers to be enumerated, but right now it's not necessary.

	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = true, ConfigurationName = "DicomExplorerAutomation", Namespace = AutomationNamespace.Value)]
	public class DicomExplorerAutomation : IDicomExplorerAutomation
	{
		public DicomExplorerAutomation()
		{
		}

		#region IDicomExplorerAutomation Members

		public SearchLocalStudiesResult SearchLocalStudies(SearchLocalStudiesRequest request)
		{
			if (request == null)
				throw new FaultException("The request cannot be null.");
			
			if (!DicomExplorerComponent.HasLocalDatastoreSupport())
				throw new FaultException<NoLocalStoreFault>(new NoLocalStoreFault(), "No local store was found.");

			DicomExplorerComponent explorerComponent = GetDicomExplorer();

			if (request.SearchCriteria == null)
				request.SearchCriteria = new DicomExplorerSearchCriteria();

			//Select the local data store node.
			explorerComponent.ServerTreeComponent.SetSelection(explorerComponent.ServerTreeComponent.ServerTree.RootNode.LocalDataStoreNode);

			SetSearchCriteria(explorerComponent.SearchPanelComponent, request.SearchCriteria);

			SynchronizationContext.Current.Post(delegate { explorerComponent.SearchPanelComponent.Search(); }, null); 

			return new SearchLocalStudiesResult();
		}

		public SearchRemoteStudiesResult SearchRemoteStudies(SearchRemoteStudiesRequest request)
		{
			if (request == null)
				throw new FaultException("The request cannot be null.");

			DicomExplorerComponent explorerComponent = GetDicomExplorer();

			if (request.SearchCriteria == null)
				request.SearchCriteria = new DicomExplorerSearchCriteria();

			string aeTitle = (request.AETitle ?? "").Trim();
			if (String.IsNullOrEmpty(aeTitle))
			{
				explorerComponent.SelectDefaultServers();
			}
			else
			{
				Server server = CollectionUtils.SelectFirst(explorerComponent.ServerTreeComponent.ServerTree.FindChildServers(),
													 delegate(IServerTreeNode node)
													 {
														 if (node is Server)
															 return ((Server)node).AETitle == aeTitle;

														 return false;
													 }) as Server;
				if (server == null)
					throw new FaultException<ServerNotFoundFault>(new ServerNotFoundFault(), String.Format("Server '{0}' not found.", aeTitle));

				explorerComponent.ServerTreeComponent.SetSelection(server);
			}

			SetSearchCriteria(explorerComponent.SearchPanelComponent, request.SearchCriteria);

			SynchronizationContext.Current.Post(delegate { explorerComponent.SearchPanelComponent.Search(); }, null); 
			
			return new SearchRemoteStudiesResult();
		}

		#endregion

		private static DicomExplorerComponent GetDicomExplorer()
		{
			List<DicomExplorerComponent> explorerComponents = DicomExplorerComponent.GetActiveComponents();
			if (explorerComponents.Count == 0)
				throw new FaultException<DicomExplorerNotFoundFault>(new DicomExplorerNotFoundFault(), "No dicom explorers were found.");

			IDesktopWindow parentDesktopWindow;
			IDesktopObject parentShelfOrWorkspace;
			GetOwnerWindows(explorerComponents[0], out parentDesktopWindow, out parentShelfOrWorkspace);
			if (parentDesktopWindow != null) //activate the owner, if it was found.
				parentDesktopWindow.Activate();
			if (parentShelfOrWorkspace != null)
				parentShelfOrWorkspace.Activate();

			//there's only ever one of these right now anyway.
			return explorerComponents[0];
		}

		private static void GetOwnerWindows(DicomExplorerComponent explorerComponent, 
			out IDesktopWindow parentDesktopWindow, out IDesktopObject parentShelfOrWorkspace)
		{
			parentDesktopWindow = null;
			parentShelfOrWorkspace = null;

			foreach (IDesktopWindow desktopWindow in Application.DesktopWindows)
			{
				foreach (IWorkspace workspace in desktopWindow.Workspaces)
				{
					if (workspace.Component == explorerComponent)
					{
						parentDesktopWindow = desktopWindow;
						parentShelfOrWorkspace = workspace;
						return;
					}
				}

				foreach (IShelf shelf in desktopWindow.Shelves)
				{
					if (shelf.Component == explorerComponent)
					{
						parentDesktopWindow = desktopWindow;
						parentShelfOrWorkspace = shelf;
						return;
					}
				}
			}
		}

		private static string GetFirstDefaultServerAETitle()
		{
			List<Server> defaultServers = DefaultServers.GetAll();

			//since streaming servers are queried automatically, it's more likely users will
			//want to query non-streaming servers.
			foreach (Server server in defaultServers)
			{
				if (!server.IsStreaming)
					return server.AETitle;
			}

			foreach (Server server in defaultServers)
				return server.AETitle;

			return null;
		}

		private static void SetSearchCriteria(SearchPanelComponent searchPanel, DicomExplorerSearchCriteria searchCriteria)
		{
			searchPanel.PatientID = searchCriteria.PatientId;
			searchPanel.PatientsName = searchCriteria.PatientsName;
			searchPanel.AccessionNumber = searchCriteria.AccessionNumber;
			searchPanel.StudyDateFrom = searchCriteria.StudyDateFrom;
			searchPanel.StudyDateTo = searchCriteria.StudyDateTo;
			searchPanel.StudyDescription = searchCriteria.StudyDescription;
			searchPanel.ReferringPhysiciansName = searchCriteria.ReferringPhysiciansName;
			searchPanel.SearchModalities = searchCriteria.Modalities ?? new List<string>();
		}
	}
}
