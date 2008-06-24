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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Enumerations", "Launch")]
	[ActionPermission("launch", AuthorityTokens.Admin.Data.Enumeration)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class EnumerationAdminTool : Tool<IDesktopToolContext>
    {
        private Workspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    EnumerationSummaryComponent component = new EnumerationSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleEnumerationAdmin);
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
    /// Extension point for views onto <see cref="EnumerationSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class EnumerationSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// EnumerationSummaryComponent class
    /// </summary>
    [AssociateView(typeof(EnumerationSummaryComponentViewExtensionPoint))]
	public class EnumerationSummaryComponent : SummaryComponentBase<EnumValueInfo, EnumValueInfoTable>
    {
        private List<EnumerationSummary> _enumerations;
        private EnumerationSummary _selectedEnumeration;

        /// <summary>
        /// Constructor
        /// </summary>
        public EnumerationSummaryComponent()
        {
			_enumerations = new List<EnumerationSummary>();
        }

        public override void Start()
        {
            Platform.GetService<IEnumerationAdminService>(
                delegate(IEnumerationAdminService service)
                {
                    ListEnumerationsResponse response = service.ListEnumerations(new ListEnumerationsRequest());
                    _enumerations = response.Enumerations;
                    _enumerations.Sort(delegate(EnumerationSummary x, EnumerationSummary y) { return x.DisplayName.CompareTo(y.DisplayName); });
                });

            _selectedEnumeration = CollectionUtils.FirstElement(_enumerations);

            base.Start();
        }

        #region Presentation Model

        public List<string> EnumerationChoices
        {
            get
            {
                return CollectionUtils.Map<EnumerationSummary, string, List<string>>(_enumerations,
                    delegate(EnumerationSummary s) { return s.DisplayName; });
            }
        }

        public string SelectedEnumeration
        {
            get { return _selectedEnumeration.DisplayName; }
            set
            {
                EnumerationSummary summary = CollectionUtils.SelectFirst(_enumerations,
                    delegate(EnumerationSummary s) { return s.DisplayName == value; });

                if (_selectedEnumeration != summary)
                {
                    _selectedEnumeration = summary;

					LoadEnumerationValues();

                    NotifyPropertyChanged("SelectedEnumeration");
                    NotifyPropertyChanged("SelectedEnumerationClassName");
                }
            }
        }

        public string SelectedEnumerationClassName
        {
            get { return _selectedEnumeration == null ? null : _selectedEnumeration.AssemblyQualifiedClassName; }
        }

        #endregion

		#region Overrides

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(CrudActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(AuthorityTokens.Admin.Data.Enumeration);
			model.Edit.SetPermissibility(AuthorityTokens.Admin.Data.Enumeration);
			model.Delete.SetPermissibility(AuthorityTokens.Admin.Data.Enumeration);
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
		protected override IList<EnumValueInfo> ListItems(int firstItem, int maxItems)
		{
			ListEnumerationValuesResponse listResponse = new ListEnumerationValuesResponse();
			if (_selectedEnumeration != null)
			{
				Platform.GetService<IEnumerationAdminService>(
					delegate(IEnumerationAdminService service)
					{
						listResponse = service.ListEnumerationValues(new ListEnumerationValuesRequest(_selectedEnumeration.AssemblyQualifiedClassName));
					});
			}

			return listResponse.Values;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<EnumValueInfo> addedItems)
		{
			// Assign value to addedItems, but we actually don't use this
			// because the entire table need to be refreshed after changes to any enumValueInfo item
			addedItems = new List<EnumValueInfo>();

			EnumerationEditorComponent component = new EnumerationEditorComponent(
				_selectedEnumeration.AssemblyQualifiedClassName,
				this.Table.Items);

			ApplicationComponentExitCode result = LaunchAsDialog(this.Host.DesktopWindow, component, SR.TitleEnumAddValue);
			if (result == ApplicationComponentExitCode.Accepted)
			{
				// refresh entire table
				LoadEnumerationValues();
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
		protected override bool EditItems(IList<EnumValueInfo> items, out IList<EnumValueInfo> editedItems)
		{
			// Assign value to addedItems, but we actually don't use this
			// because the entire table need to be refreshed after changes to any enumValueInfo item
			editedItems = new List<EnumValueInfo>();

			EnumValueInfo item = CollectionUtils.FirstElement(items);
			EnumerationEditorComponent component = new EnumerationEditorComponent(
				_selectedEnumeration.AssemblyQualifiedClassName,
				(EnumValueInfo)item.Clone(),
				this.Table.Items);
			ApplicationComponentExitCode result = LaunchAsDialog(this.Host.DesktopWindow, component, SR.TitleEnumEditValue + " - " + item.Code);
			if (result == ApplicationComponentExitCode.Accepted)
			{
				// refresh entire table
				LoadEnumerationValues();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<EnumValueInfo> items)
		{
			foreach (EnumValueInfo item in items)
			{
				Platform.GetService<IEnumerationAdminService>(
					delegate(IEnumerationAdminService service)
					{
						EnumValueInfo valueToDelete = item;
						service.RemoveValue(new RemoveValueRequest(_selectedEnumeration.AssemblyQualifiedClassName, valueToDelete));
					});
			}

			return true;
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(EnumValueInfo x, EnumValueInfo y)
		{
			return Equals(x.Code, y.Code);
		}

		protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();

			// overriding base behaviour
			if (_selectedEnumeration == null)
			{
				this.ActionModel.Add.Enabled = false;
				this.ActionModel.Delete.Enabled = false;
				this.ActionModel.Edit.Enabled = false;
			}
			else
			{
				this.ActionModel.Add.Enabled = this.ActionModel.Add.Enabled && _selectedEnumeration.CanAddRemoveValues;
				this.ActionModel.Delete.Enabled = this.ActionModel.Delete.Enabled && _selectedEnumeration.CanAddRemoveValues;
			}

		}

		#endregion

		private void LoadEnumerationValues()
		{
			this.Table.Items.Clear();
			this.Table.Items.AddRange(this.PagingController.GetFirst());
		}
	}
}
