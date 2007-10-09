using System;
using System.Collections;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract class that roughly represents the notion of an IHE/DICOM General Purpose Performed Procedure Step (GP-PPS)
    /// </summary>
    public abstract class PerformedProcedureStep : PerformedStep
    {
        private string _documentation;
        private IDictionary _extendedProperties = new Hashtable();


        public PerformedProcedureStep(Staff performingStaff)
            : base(new ProcedureStepPerformer(performingStaff))
        {
        }

        public PerformedProcedureStep(Staff performingStaff, string documentation)
            : base(new ProcedureStepPerformer(performingStaff))
        {
            _documentation = documentation;
        }

        public PerformedProcedureStep()
        {
        }

        /// <summary>
        /// Placeholder for Documentation pending introduction of "tagging"
        /// </summary>
        public string Documentation
        {
            get { return _documentation; }
            set { _documentation = value; }
        }

        public IDictionary ExtendedProperties
        {
            get { return _extendedProperties; }
            private set { _extendedProperties = value; }
        }

    }
}
