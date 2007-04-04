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
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Staff")]
    [ClickHandler("launch", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.StaffAdmin)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class StaffSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                StaffSummaryComponent component = new StaffSummaryComponent();

                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    component,
                    SR.TitleStaff,
                    delegate(IApplicationComponent c) { _workspace = null; });
            }
            else
            {
                _workspace.Activate();
            }
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="StaffSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class StaffSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// StaffSummaryComponent class
    /// </summary>
    [AssociateView(typeof(StaffSummaryComponentViewExtensionPoint))]
    public class StaffSummaryComponent : ApplicationComponent
    {
        private StaffSummary _selectedStaff;
        private StaffTable _staffTable;

        private ActionModelRoot _staffActionHandler;
        private ClickAction _addStaffAction;
        private ClickAction _addPractitionerAction;
        private ClickAction _editAction;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffSummaryComponent()
        {
        }

        public override void Start()
        {
            //_staffAdminService.StaffChanged += StaffChangedEventHandler;
            //_staffAdminService.PractitionerChanged += PractitionerChangedEventHandler;

            _staffTable = new StaffTable();
            _staffActionHandler = new ActionModelRoot();
            _addPractitionerAction = CreateAction(SR.TitleAddPractitioner, "Icons.Add.png", AddPractitioner);
            _addStaffAction = CreateAction(SR.TitleAddStaff, "Icons.Add.png", AddStaff);
            _editAction = CreateAction(SR.TitleUpdateStaff, "Icons.Edit.png", UpdateSelectedStaff);
            _staffActionHandler.InsertAction(_addPractitionerAction);
            _staffActionHandler.InsertAction(_addStaffAction);
            _staffActionHandler.InsertAction(_editAction);
            
            base.Start();
        }

        public override void Stop()
        {
            //_staffAdminService.StaffChanged -= StaffChangedEventHandler;
            //_staffAdminService.PractitionerChanged -= PractitionerChangedEventHandler;

            base.Stop();
        }

        #region Event Handler

        //TODO: PractitionerChangedEventHandler
        //private void PractitionerChangedEventHandler(object sender, EntityChangeEventArgs e)
        //{
        //    // check if the staff with this oid is in the list
        //    int index = _staffTable.Items.FindIndex(delegate(Staff s) { return e.EntityRef.RefersTo(s); });
        //    IPractitionerAdminService practitionerAdminService = ApplicationContext.GetService<IPractitionerAdminService>();
        //    if (index > -1)
        //    {
        //        if (e.ChangeType == EntityChangeType.Update)
        //        {
        //            try
        //            {
        //                Practitioner p = practitionerAdminService.LoadPractitioner((EntityRef<Practitioner>)e.EntityRef, false);
        //                _staffTable.Items[index] = p;
        //            }
        //            catch (Exception exception)
        //            {
        //                ExceptionHandler.Report(exception, this.Host.DesktopWindow);
        //            }
        //        }
        //        else if (e.ChangeType == EntityChangeType.Delete)
        //        {
        //            _staffTable.Items.RemoveAt(index);
        //        }
        //    }
        //    else
        //    {
        //        if (e.ChangeType == EntityChangeType.Create)
        //        {
        //            try
        //            {
        //                Practitioner p = practitionerAdminService.LoadPractitioner((EntityRef<Practitioner>)e.EntityRef, false);
        //                if (p != null)
        //                    _staffTable.Items.Add(p);
        //            }
        //            catch (Exception exception)
        //            {
        //                ExceptionHandler.Report(exception, this.Host.DesktopWindow);
        //            }
        //        }
        //    }
        //}

        //TODO: StaffChangedEventHandler
        //private void StaffChangedEventHandler(object sender, EntityChangeEventArgs e)
        //{
        //    // check if the staff with this oid is in the list
        //    int index = _staffTable.Items.FindIndex(delegate(Staff s) { return e.EntityRef.RefersTo(s); });
        //    if (index > -1)
        //    {
        //        if (e.ChangeType == EntityChangeType.Update)
        //        {
        //            try
        //            {
        //                Staff s = _staffAdminService.LoadStaff((EntityRef<Staff>)e.EntityRef, false);
        //                _staffTable.Items[index] = s;
        //            }
        //            catch (Exception exception)
        //            {
        //                ExceptionHandler.Report(exception, this.Host.DesktopWindow);
        //            }
        //        }
        //        else if (e.ChangeType == EntityChangeType.Delete)
        //        {
        //            _staffTable.Items.RemoveAt(index);
        //        }
        //    }
        //    else
        //    {
        //        if (e.ChangeType == EntityChangeType.Create)
        //        {
        //            try
        //            {
        //                Staff s = _staffAdminService.LoadStaff((EntityRef<Staff>)e.EntityRef, false);
        //                if (s != null)
        //                    _staffTable.Items.Add(s);
        //            }
        //            catch (Exception exception)
        //            {
        //                ExceptionHandler.Report(exception, this.Host.DesktopWindow);
        //            }
        //        }
        //    }
        //}

        #endregion

        #region Presentation Model

        public ITable Staffs
        {
            get { return _staffTable; }
        }

        public ActionModelNode StaffListActionModel
        {
            get { return _staffActionHandler; }
        }

        public ISelection SelectedStaff
        {
            get { return _selectedStaff == null ? Selection.Empty : new Selection(_selectedStaff); }
            set
            {
                _selectedStaff = (StaffSummary)value.Item;
                StaffSelectionChanged();
            }
        }

        public void AddStaff()
        {
            StaffEditorComponent editor = new StaffEditorComponent(true);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleAddStaff);
        }

        public void AddPractitioner()
        {
            StaffEditorComponent editor = new StaffEditorComponent(false);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow, editor, SR.TitleAddPractitioner);
        }

        public void UpdateSelectedStaff()
        {
            // can occur if user double clicks while holding control
            if (_selectedStaff == null) return;

            StaffEditorComponent editor;
            if (_selectedStaff.LicenseNumber == null || _selectedStaff.LicenseNumber == "")
            {
                editor = new StaffEditorComponent(_selectedStaff.StaffRef, true);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleUpdateStaff);
            }
            else
            {
                editor = new StaffEditorComponent(_selectedStaff.StaffRef, false);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleUpdatePractitioner);
            }
        }

        public void LoadStaffTable()
        {
            try
            {
                Platform.GetService<IStaffAdminService>(
                    delegate(IStaffAdminService service)
                    {
                        ListAllStaffsResponse response = service.ListAllStaffs(new ListAllStaffsRequest());
                        if (response.Staffs != null)
                        {
                            _staffTable.Items.Clear();
                            _staffTable.Items.AddRange(response.Staffs);
                        }
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        private void StaffSelectionChanged()
        {
            if (_selectedStaff != null)
                _editAction.Enabled = true;
            else
                _editAction.Enabled = false;
        }

        private ClickAction CreateAction(string name, string icon, ClickHandlerDelegate handler)
        {
            IResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
            ClickAction action = new ClickAction(name, new ActionPath(string.Format("root/{0}", name), resolver), ClickActionFlags.None, resolver);
            action.Tooltip = name;
            action.Label = name;
            action.IconSet = new IconSet(IconScheme.Colour, icon, icon, icon);
            action.Enabled = true;
            action.SetClickHandler(handler);

            return action;
        }
    }
}