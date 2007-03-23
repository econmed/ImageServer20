using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="VisitPractitionerSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class VisitPractitionerSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// VisitPractitionerSummaryComponent class
    /// </summary>
    [AssociateView(typeof(VisitPractitionerSummaryComponentViewExtensionPoint))]
    public class VisitPractitionersSummaryComponent : ApplicationComponent
    {
        private VisitDetail _visit;
        private VisitPractitionerDetail _currentVisitPractitionerSelection;
        private VisitPractitionerTable _practitionersTable;
        private List<EnumValueInfo> _visitPractitionerRoleChoices;
        private CrudActionModel _visitPractitionerActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitPractitionersSummaryComponent(List<EnumValueInfo> visitPractitionerRoleChoices)
        {
            _practitionersTable = new VisitPractitionerTable();
            _visitPractitionerRoleChoices = visitPractitionerRoleChoices;

            _visitPractitionerActionHandler = new CrudActionModel();
            _visitPractitionerActionHandler.Add.SetClickHandler(AddVisitPractitioner);
            _visitPractitionerActionHandler.Edit.SetClickHandler(UpdateSelectedVisitPractitioner);
            _visitPractitionerActionHandler.Delete.SetClickHandler(DeleteSelectedVisitPractitioner);

            _visitPractitionerActionHandler.Add.Enabled = true;
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public VisitDetail Visit
        {
            get { return _visit; }
            set { _visit = value; }
        }

        public ITable VisitPractitioners
        {
            get { return _practitionersTable; }
        }

        public ActionModelNode VisitPractionersActionModel
        {
            get { return _visitPractitionerActionHandler; }
        }

        public VisitPractitionerDetail CurrentVisitPractitionerSelection
        {
            get { return _currentVisitPractitionerSelection; }
            set
            {
                _currentVisitPractitionerSelection = value;
                VisitPractitionerSelectionChanged();
            }
        }

        public void SetSelectedVisitPractitioner(ISelection selection)
        {
            this.CurrentVisitPractitionerSelection = (VisitPractitionerDetail)selection.Item;
        }

        private void VisitPractitionerSelectionChanged()
        {
            if (_currentVisitPractitionerSelection != null)
            {
                _visitPractitionerActionHandler.Edit.Enabled = true;
                _visitPractitionerActionHandler.Delete.Enabled = true;
            }
            else
            {
                _visitPractitionerActionHandler.Edit.Enabled = false;
                _visitPractitionerActionHandler.Delete.Enabled = false;
            }
        }

        public void AddVisitPractitioner()
        {
            DummyAddVisitPractitioner();

            LoadVisitPractioners();
            this.Modified = true;
        }

        public void UpdateSelectedVisitPractitioner()
        {
            DummyUpdateVisitPractitioner();

            LoadVisitPractioners();
            this.Modified = true;
        }

        public void DeleteSelectedVisitPractitioner()
        {
            _visit.Practitioners.Remove(_currentVisitPractitionerSelection);

            LoadVisitPractioners();
            this.Modified = true;
        }

        public void LoadVisitPractioners()
        {
            _practitionersTable.Items.Clear();
            _practitionersTable.Items.AddRange(_visit.Practitioners);
        }

        #region Dummy Code

        private void DummyAddVisitPractitioner()
        {
            try
            {
                Platform.GetService<IPractitionerAdminService>(
                    delegate(IPractitionerAdminService service)
                    {
                        FindPractitionersResponse findResponse = service.FindPractitioners(new FindPractitionersRequest("Who", ""));
                        PractitionerSummary practitioner;
                        if (findResponse.Practitioners.Count == 0)
                        {
                            AddPractitionerResponse addResponse = service.AddPractitioner(new AddPractitionerRequest(new PractitionerDetail(
                                    new PersonNameDetail("Who", "Doctor", "", "", "", ""),
                                    null, null, "123456")));
                            practitioner = addResponse.Practitioner;
                        }
                        else
                        {
                            practitioner = findResponse.Practitioners[0];
                        }

                        VisitPractitionerDetail vp = new VisitPractitionerDetail();

                        vp.Role = CollectionUtils.SelectFirst<EnumValueInfo>(_visitPractitionerRoleChoices,
                                delegate(EnumValueInfo e) { return e.Code == "RF"; });
                        vp.Practitioner = practitioner;
                        vp.StartTime = Platform.Time;

                        _visit.Practitioners.Add(vp);
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void DummyUpdateVisitPractitioner()
        {
            VisitPractitionerDetail vp = _currentVisitPractitionerSelection;

            vp.Role = CollectionUtils.SelectFirst<EnumValueInfo>(_visitPractitionerRoleChoices,
                    delegate(EnumValueInfo e) { return e.Code == "CN"; });
            vp.EndTime = Platform.Time;
        }
    	#endregion    
    }
}
