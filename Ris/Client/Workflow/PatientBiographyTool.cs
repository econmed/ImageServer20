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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ButtonAction("view", "folderexplorer-items-toolbar/Patient Biography", "View")]
	[MenuAction("view", "folderexplorer-items-contextmenu/Patient Biography", "View")]
	[ButtonAction("view", "patientsearch-items-toolbar/Patient Biography", "View")]
	[MenuAction("view", "patientsearch-items-contextmenu/Patient Biography", "View")]
	[EnabledStateObserver("view", "Enabled", "EnabledChanged")]
	[Tooltip("view", "Open patient biography")]
	[IconSet("view", IconScheme.Colour, "PatientDetailsToolSmall.png", "PatientDetailsToolMedium.png", "PatientDetailsToolLarge.png")]
	[ActionPermission("view", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.PatientBiography.View)]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(EmergencyWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(OrderNoteboxItemToolExtensionPoint))]
	[ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class PatientBiographyTool : Tool<IToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;
		
		public override void Initialize()
		{
			base.Initialize();

			if (this.ContextBase is IWorkflowItemToolContext)
			{
				((IWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
			else if (this.ContextBase is IPatientSearchToolContext)
			{
				((IPatientSearchToolContext)this.ContextBase).SelectedProfileChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
		}

		private bool DetermineEnablement()
		{
			if (this.ContextBase is IWorkflowItemToolContext)
			{
				var ctx = (IWorkflowItemToolContext) this.ContextBase;
				return ctx.Selection != null && ctx.Selection.Items.Length == 1;
			}
			else if (this.ContextBase is IPatientSearchToolContext)
			{
				var context = (IPatientSearchToolContext)this.ContextBase;
				return (context.SelectedProfile != null && context.SelectedProfile.PatientProfileRef != null);
			}

			return false;
		}

		public bool Enabled
		{
			get
			{
				this.Enabled = DetermineEnablement();
				return _enabled;
			}
			set
			{
				if (_enabled == value)
					return;

				_enabled = value;
				EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public void View()
		{
			if(this.ContextBase is IWorkflowItemToolContext)
			{
				var context = (IWorkflowItemToolContext)ContextBase;
				if (this.Context is IOrderNoteboxItemToolContext)
				{
					var item = (OrderNoteboxItemSummary)context.Selection.Item;
					OpenPatient(item.PatientRef, item.PatientProfileRef, item.OrderRef, context.DesktopWindow);
				}
				else
				{
					var item = (WorklistItemSummaryBase) context.Selection.Item;
					OpenPatient(item.PatientRef, item.PatientProfileRef, item.OrderRef, context.DesktopWindow);
				}
			}
			else if (this.ContextBase is IPatientSearchToolContext)
			{
				var context = (IPatientSearchToolContext)this.ContextBase;
				var profile = context.SelectedProfile;
				OpenPatient(profile.PatientRef, profile.PatientProfileRef, null, context.DesktopWindow);
			}
		}

		protected static void OpenPatient(EntityRef patientRef, EntityRef profileRef, EntityRef orderRef, IDesktopWindow window)
		{
			try
			{
				var document = DocumentManager.Get<PatientBiographyDocument>(profileRef);
				if (document == null)
				{
					document = new PatientBiographyDocument(patientRef, profileRef, orderRef, window);
					document.Open();
				}
				else
				{
					document.Open();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, window);
			}
		}
	}
}
