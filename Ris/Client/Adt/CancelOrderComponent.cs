using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="CancelOrderComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class CancelOrderComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// CancelOrderComponent class
    /// </summary>
    [AssociateView(typeof(CancelOrderComponentViewExtensionPoint))]
    public class CancelOrderComponent : ApplicationComponent
    {
        private EntityRef _patientProfileRef;
        private CancelOrderTable _cancelOrderTable;

        private EnumValueInfo _selectedCancelReason;
        private List<EnumValueInfo> _cancelReasonChoices;

        private List<EntityRef> _selectedOrders;

        /// <summary>
        /// Constructor
        /// </summary>
        public CancelOrderComponent(EntityRef patientProfileRef)
        {
            _patientProfileRef = patientProfileRef;
        }

        public override void Start()
        {
            _selectedOrders = new List<EntityRef>();
            _cancelOrderTable = new CancelOrderTable();

            try
            {
                Platform.GetService<IRegistrationWorkflowService>(
                    delegate(IRegistrationWorkflowService service)
                    {
                        GetDataForCancelOrderTableResponse response = service.GetDataForCancelOrderTable(new GetDataForCancelOrderTableRequest(_patientProfileRef));
                        _cancelOrderTable.Items.AddRange(
                            CollectionUtils.Map<CancelOrderTableItem, CancelOrderTableEntry>(response.CancelOrderTableItems,
                                    delegate(CancelOrderTableItem item)
                                    {
                                        CancelOrderTableEntry entry = new CancelOrderTableEntry(item);
                                        entry.CheckedChanged += new EventHandler(CancelOrderCheckedStateChangedEventHandler);
                                        return entry;
                                    }));


                        _cancelReasonChoices = response.CancelReasonChoices;
                        _selectedCancelReason = _cancelReasonChoices[0];
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public ITable CancelOrderTable
        {
            get { return _cancelOrderTable; }
        }

        public List<string> CancelReasonChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_cancelReasonChoices); }
        }

        public string SelectedCancelReason
        {
            get { return _selectedCancelReason == null ? "" : _selectedCancelReason.Value; }
            set
            {
                _selectedCancelReason = (value == "") ? null :
                    CollectionUtils.SelectFirst<EnumValueInfo>(_cancelReasonChoices,
                        delegate(EnumValueInfo reason) { return reason.Value == value; });
            }
        }

        public EnumValueInfo SelectedReason
        {
            get { return _selectedCancelReason; }
        }

        public List<EntityRef> SelectedOrders
        {
            get { return _selectedOrders; }
        }

        #endregion

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                try
                {
                    SaveChanges();
                    this.ExitCode = ApplicationComponentExitCode.Normal;
                    Host.Exit();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }
        }

        private void SaveChanges()
        {
            // Get the list of Order EntityRef from the table
            foreach (CancelOrderTableEntry entry in _cancelOrderTable.Items)
            {
                if (entry.Checked)
                    _selectedOrders.Add(entry.CancelOrderTableItem.OrderRef);
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        private void CancelOrderCheckedStateChangedEventHandler(object sender, EventArgs e)
        {
            foreach (CancelOrderTableEntry entry in _cancelOrderTable.Items)
            {
                if (entry.Checked)
                {
                    this.Modified = true;
                    return;
                }
            }

            this.Modified = false;
        }
    
    }
}
