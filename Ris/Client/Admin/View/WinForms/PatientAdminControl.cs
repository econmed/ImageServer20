using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    public partial class PatientAdminControl : UserControl
    {
        public PatientAdminControl()
        {
            InitializeComponent();
        }

        public TableView PatientTableView
        {
            get { return _patientTableView; }
        }
    }
}
