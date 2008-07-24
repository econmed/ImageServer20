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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;

using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using ClearCanvas.Ris.Application.Common.Admin.LocationAdmin;
using ClearCanvas.Ris.Application.Common;
using System.Collections;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Locations", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Location)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class LocationSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    LocationSummaryComponent component = new LocationSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleLocations);
                    _workspace.Closed += delegate { _workspace = null; };

                }
                catch (Exception e)
                {
                    // could not launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _workspace.Activate();
            }
        }
    }
    
    /// <summary>
    /// Extension point for views onto <see cref="LocationSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class LocationSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// LocationSummaryComponent class
    /// </summary>
	[AssociateView(typeof(LocationSummaryComponentViewExtensionPoint))]
    public class LocationSummaryComponent : SummaryComponentBase<LocationSummary, LocationTable>
    {
		private FacilitySummary _filterNone = new FacilitySummary();
		private ArrayList _facilityList = new ArrayList();
		private FacilitySummary _facility;
		private string _name;

		public override void Start()
		{
			Platform.GetService<ILocationAdminService>(
				delegate(ILocationAdminService service)
				{
					GetLocationEditFormDataResponse response = service.GetLocationEditFormData(new GetLocationEditFormDataRequest());
					_filterNone.Name = SR.DummyItemNone;
					_facilityList.Add(_filterNone);
					_facilityList.AddRange(response.FacilityChoices);
				});
			base.Start();
			_facility = _filterNone;
		}

		# region Presentation Model

		public object NullFilter
		{
			get { return _filterNone; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public object Facility
		{
			get {return _facility; }
			set
			{
				if (value == _filterNone)
					_facility = null;
				else
					_facility = (FacilitySummary)value;

				Search();
				this.Modified = true;
				if (value == _filterNone)
					_facility = _filterNone;
			}
		}

		public IList FacilityChoices
		{
			get { return _facilityList; }
		}

		public string FormatFacilityListItem(object item)
		{
			FacilitySummary summary = (FacilitySummary)item;
			return string.Format(summary.Name);
		}

		#endregion

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Location);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Location);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Location);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Location);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <param name="firstItem"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		protected override IList<LocationSummary> ListItems(int firstItem, int maxItems)
		{
			ListAllLocationsResponse listResponse = null;
			Platform.GetService<ILocationAdminService>(
				delegate(ILocationAdminService service)
				{
					ListAllLocationsRequest request = new ListAllLocationsRequest(new SearchResultPage(firstItem, maxItems));
					if(_facility != _filterNone && _facility != null)
						request.Facility = _facility;
					request.Name = _name;
					listResponse = service.ListAllLocations(request);
				});

			return listResponse.Locations;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<LocationSummary> addedItems)
		{
			addedItems = new List<LocationSummary>();
			LocationEditorComponent editor = new LocationEditorComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddLocation);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.LocationSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "edit" action.
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected override bool EditItems(IList<LocationSummary> items, out IList<LocationSummary> editedItems)
		{
			editedItems = new List<LocationSummary>();
			LocationSummary item = CollectionUtils.FirstElement(items);

			LocationEditorComponent editor = new LocationEditorComponent(item.LocationRef);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleUpdateLocation + " - " + "("+item.Id+") " +item.Name);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.LocationSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <param name="failureMessage">The message if there any errors that occurs during deletion.</param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<LocationSummary> items, out IList<LocationSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<LocationSummary>();

			foreach (LocationSummary item in items)
			{
				try
				{
					Platform.GetService<ILocationAdminService>(
						delegate(ILocationAdminService service)
						{
							service.DeleteLocation(new DeleteLocationRequest(item.LocationRef));
						});

					deletedItems.Add(item);
				}
				catch (Exception e)
				{
					failureMessage = e.Message;
				}
			}

			return deletedItems.Count > 0;
		}

		/// <summary>
		/// Called to handle the "toggle activation" action, if supported
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected override bool UpdateItemsActivation(IList<LocationSummary> items, out IList<LocationSummary> editedItems)
		{
			List<LocationSummary> results = new List<LocationSummary>();
			foreach (LocationSummary item in items)
			{
				Platform.GetService<ILocationAdminService>(
					delegate(ILocationAdminService service)
					{
						LocationDetail detail = service.LoadLocationForEdit(
							new LoadLocationForEditRequest(item.LocationRef)).LocationDetail;
						detail.Deactivated = !detail.Deactivated;
						LocationSummary summary = service.UpdateLocation(
							new UpdateLocationRequest(detail)).Location;

						results.Add(summary);
					});
			}

			editedItems = results;
			return true;
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(LocationSummary x, LocationSummary y)
		{
			return x.LocationRef.Equals(y.LocationRef, true);
		}

    }
}
