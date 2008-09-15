using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class Startup : Form
    {
        public Startup()
        {
            InitializeComponent();
        }


        private void TestRule_Click(object sender, EventArgs e)
        {
            TestDicomFileForm test = new TestDicomFileForm();
            test.Show();
        }

        private void TestHeaderStreamButton_Click(object sender, EventArgs e)
        {
            TestHeaderStreamingForm test = new TestHeaderStreamingForm();
            test.Show();
        }

        private void buttonCompression_Click(object sender, EventArgs e)
        {
            TestCompressionForm test = new TestCompressionForm();
            test.Show();
        }

        private void buttonEditStudy_Click(object sender, EventArgs e)
        {
            TestEditStudyForm test = new TestEditStudyForm();
            test.Show();
        }

        private void TestReconcileButton_Click(object sender, EventArgs e)
        {
            TestReconcileForm test = new TestReconcileForm();
            test.Show();
        }
    }
}