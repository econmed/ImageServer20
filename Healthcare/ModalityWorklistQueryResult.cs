using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    public class ModalityWorklistQueryResult
    {
        private EntityRef<Patient> _patient;
        private EntityRef<PatientProfile> _patientProfile;
        private EntityRef<Order> _order;
        private EntityRef<RequestedProcedure> _requestedProcedure;
        private EntityRef<ModalityProcedureStep> _procedureStep;

        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private CompositeIdentifier _visitNumber;
        private string _accessionNumber;
        private string _diagnosticServiceName;
        private string _requestedProcedureName;
        private string _procedureStepName;
        private string _modalityName;
        private OrderPriority _priority;
        private ActivityStatus _status;

        public ModalityWorklistQueryResult(
            Patient patient,
            PatientProfile profile,
            Order order,
            RequestedProcedure requestedProcedure,
            ModalityProcedureStep procedureStep,
            CompositeIdentifier mrn,
            PersonName patientName,
            CompositeIdentifier visitNumber,
            string accessionNumber,
            string diagnosticService,
            string requestedProcedureName,
            string procedureStepName,
            string modalityName,
            OrderPriority priority,
            ActivityStatus status)
        {
            _patient = new EntityRef<Patient>(patient);
            _patientProfile = new EntityRef<PatientProfile>(profile);
            _order = new EntityRef<Order>(order);
            _requestedProcedure = new EntityRef<RequestedProcedure>(requestedProcedure);
            _procedureStep = new EntityRef<ModalityProcedureStep>(procedureStep);

            _mrn = mrn;
            _patientName = patientName;
            _visitNumber = visitNumber;
            _accessionNumber = accessionNumber;
            _diagnosticServiceName = diagnosticService;
            _requestedProcedureName = requestedProcedureName;
            _procedureStepName = procedureStepName;
            _modalityName = modalityName;
            _priority = priority;
            _status = status;
        }

        public EntityRef<ModalityProcedureStep> ProcedureStep
        {
            get { return _procedureStep; }
        }

        public EntityRef<Patient> Patient
        {
            get { return _patient; }
        }

        public EntityRef<PatientProfile> PatientProfile
        {
            get { return _patientProfile; }
        }

        public EntityRef<Order> Order
        {
            get { return _order; }
        }

        public EntityRef<RequestedProcedure> RequestedProcedure
        {
            get { return _requestedProcedure; }
        }

        public CompositeIdentifier Mrn
        {
            get { return _mrn; }
        }

        public PersonName PatientName
        {
            get { return _patientName; }
        }

        public CompositeIdentifier VisitNumber
        {
            get { return _visitNumber; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
        }

        public string DiagnosticService
        {
            get { return _diagnosticServiceName; }
        }

        public string RequestedProcedureName
        {
            get { return _requestedProcedureName; }
        }

        public string ModalityProcedureStepName
        {
            get { return _procedureStepName; }
        }

        public string ModalityName
        {
            get { return _modalityName; }
        }

        public OrderPriority Priority
        {
            get { return _priority; }
        }

        public ActivityStatus Status
        {
            get { return _status; }
        }
    }
}
