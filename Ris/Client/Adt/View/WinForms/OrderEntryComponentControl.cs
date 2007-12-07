#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="OrderEntryComponent"/>
    /// </summary>
    public partial class OrderEntryComponentControl : ApplicationComponentUserControl
    {
        private OrderEntryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderEntryComponentControl(OrderEntryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            // force toolbars to be displayed (VS designer seems to have a bug with this)
            _proceduresTableView.ShowToolbar = true;
            _consultantsTableView.ShowToolbar = true;
            _notesTableView.ShowToolbar = true;
            
            _diagnosticService.LookupHandler = _component.DiagnosticServiceLookupHandler;
            _diagnosticService.DataBindings.Add("Value", _component, "SelectedDiagnosticService", true, DataSourceUpdateMode.OnPropertyChanged);
            _diagnosticService.DataBindings.Add("Enabled", _component, "IsDiagnosticServiceEditable");

            _indication.DataBindings.Add("Value", _component, "Indication", true, DataSourceUpdateMode.OnPropertyChanged);

            _proceduresTableView.Table = _component.Procedures;
            _proceduresTableView.MenuModel = _component.ProceduresActionModel;
            _proceduresTableView.ToolbarModel = _component.ProceduresActionModel;
            _proceduresTableView.DataBindings.Add("Selection", _component, "SelectedProcedure", true, DataSourceUpdateMode.OnPropertyChanged);

            _consultantsTableView.Table = _component.Consultants;
            _consultantsTableView.MenuModel = _component.ConsultantsActionModel;
            _consultantsTableView.ToolbarModel = _component.ConsultantsActionModel;
            _consultantsTableView.DataBindings.Add("Selection", _component, "SelectedConsultant", true, DataSourceUpdateMode.OnPropertyChanged);
            _addConsultantButton.DataBindings.Add("Enabled", _component.ConsultantsActionModel.Add, "Enabled");

            _consultantLookup.LookupHandler = _component.ConsultantsLookupHandler;
            _consultantLookup.DataBindings.Add("Value", _component, "ConsultantToAdd", true, DataSourceUpdateMode.OnPropertyChanged);

            _visit.DataBindings.Add("Value", _component, "SelectedVisit", true, DataSourceUpdateMode.OnPropertyChanged);
            _visit.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatVisit(e.ListItem); };
            RefreshActiveVisit();
            _component.ActiveVisitsChanged += _component_ActiveVisitsChanged;

            _priority.DataSource = _component.PriorityChoices;
            _priority.DataBindings.Add("Value", _component, "SelectedPriority", true, DataSourceUpdateMode.OnPropertyChanged);

            _orderingFacility.DataBindings.Add("Value", _component, "OrderingFacility", true, DataSourceUpdateMode.OnPropertyChanged);

            _orderingPractitioner.LookupHandler = _component.OrderingPractitionerLookupHandler;
            _orderingPractitioner.DataBindings.Add("Value", _component, "SelectedOrderingPractitioner", true, DataSourceUpdateMode.OnPropertyChanged);

            // bind date and time to same property
            _schedulingRequestDate.DataBindings.Add("Value", _component, "SchedulingRequestTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _schedulingRequestTime.DataBindings.Add("Value", _component, "SchedulingRequestTime", true, DataSourceUpdateMode.OnPropertyChanged);

            _reorderReason.DataSource = _component.CancelReasonChoices;
            _reorderReason.DataBindings.Add("Value", _component, "SelectedCancelReason", true, DataSourceUpdateMode.OnPropertyChanged);
            _reorderReason.DataBindings.Add("Visible", _component, "IsCancelReasonVisible");

            _documentTableView.Table = _component.Attachments;
            _documentTableView.MenuModel = _component.AttachmentActionModel;
            _documentTableView.ToolbarModel = _component.AttachmentActionModel;
            _documentTableView.DataBindings.Add("Selection", _component, "SelectedAttachment", true, DataSourceUpdateMode.OnPropertyChanged);
            RefreshAttachmentPreview();
            _component.SelectedAttachmentChanged += _component_AttachmentSelectionChanged;
 
        }

        void _component_ActiveVisitsChanged(object sender, EventArgs e)
        {
            RefreshActiveVisit();
        }

        void RefreshActiveVisit()
        {
            _visit.DataSource = _component.ActiveVisits;
        }

        void _component_AttachmentSelectionChanged(object sender, EventArgs e)
        {
            RefreshAttachmentPreview();
        }

        void RefreshAttachmentPreview()
        {
            if (String.IsNullOrEmpty(_component.TempFileName))
            {
                _documentBrowser.Url = new Uri("about:blank");
                return;
            }

            _documentBrowser.Url = new Uri(_component.TempFileName);
        }

        private void _placeOrderButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.Accept();
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _addConsultantButton_Click(object sender, EventArgs e)
        {
            _component.AddConsultant();
        }

        private void _proceduresTableView_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.EditSelectedProcedure();
        }

        private void _applySchedulingButton_Click(object sender, EventArgs e)
        {
            _component.ApplySchedulingToProcedures();
        }

        private void _visitSummaryButton_Click(object sender, EventArgs e)
        {
            _component.ShowVisitSummary();
        }
    }
}
