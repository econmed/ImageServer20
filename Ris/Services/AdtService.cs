using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Services
{

    [ExtensionPoint()]
    public class PatientReconciliationStrategyExtensionPoint : ExtensionPoint<IPatientReconciliationStrategy>
    {
    }

    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class AdtService : HealthcareServiceLayer, IAdtService
    {
        private IExtensionPoint _strategyExtensionPoint;

        public AdtService()
            :this(new PatientReconciliationStrategyExtensionPoint())
        {
        }

        internal AdtService(IExtensionPoint strategyExtensionPoint)
        {
            _strategyExtensionPoint = strategyExtensionPoint;
        }

        #region IAdtService Members

        [ReadOperation]
        public IList<PatientProfile> ListPatientProfiles(PatientProfileSearchCriteria criteria)
        {
            return this.CurrentContext.GetBroker<IPatientProfileBroker>().Find(criteria);
        }

        [ReadOperation]
        public Patient LoadPatientAndAllProfiles(EntityRef<PatientProfile> profileRef)
        {
            IPatientProfileBroker profileBroker = CurrentContext.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = profileBroker.Load(profileRef, EntityLoadFlags.Proxy);

            IPatientBroker patientBroker = CurrentContext.GetBroker<IPatientBroker>();
            patientBroker.LoadProfilesForPatient(profile.Patient);

            return profile.Patient;
        }

        [ReadOperation]
        public Patient LoadPatient(EntityRef<Patient> patientRef)
        {
            IPatientBroker patientBroker = CurrentContext.GetBroker<IPatientBroker>();
            return patientBroker.Load(patientRef);
        }

        [ReadOperation]
        public IList<Visit> ListPatientVisits(EntityRef<Patient> patientRef)
        {
            // ensure that the profiles collection is loaded
            IPatientBroker patientBroker = this.CurrentContext.GetBroker<IPatientBroker>();
            Patient patient = patientBroker.Load(patientRef, EntityLoadFlags.Proxy);

            VisitSearchCriteria criteria = new VisitSearchCriteria();
            criteria.Patient.EqualTo(patient);

            IVisitBroker visitBroker = this.CurrentContext.GetBroker<IVisitBroker>();
            return visitBroker.Find(criteria);
        }

        [ReadOperation]
        public void LoadVisitDetails(Visit visit)
        {
            IVisitBroker visitBroker = this.CurrentContext.GetBroker<IVisitBroker>();
            visitBroker.LoadLocationsForVisit(visit);
            visitBroker.LoadPractitionersForVisit(visit);
        }

        [ReadOperation]
        public IList<PatientProfileMatch> FindPatientReconciliationMatches(EntityRef<PatientProfile> patientProfileRef)
        {
            IPatientProfileBroker broker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            PatientProfile patientProfile = broker.Load(patientProfileRef);

            IPatientReconciliationStrategy strategy = (IPatientReconciliationStrategy)_strategyExtensionPoint.CreateExtension();
            return strategy.FindReconciliationMatches(patientProfile, broker);
        }

        [ReadOperation]
        public PatientProfile LoadPatientProfile(EntityRef<PatientProfile> profileRef, bool withDetails)
        {
            IPatientProfileBroker broker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = broker.Load(profileRef);
            if (withDetails)
            {
                broker.LoadAddressesForPatientProfile(profile);
                broker.LoadTelephoneNumbersForPatientProfile(profile);
            }
            return profile;
        }

        [UpdateOperation]
        public void ReconcilePatients(Patient destPatient, IList<Patient> sourcePatients)
        {
            IPatientBroker broker = this.CurrentContext.GetBroker<IPatientBroker>();

            // put destPatient into context
            CurrentContext.Lock(destPatient);

            foreach (Patient source in sourcePatients)
            {
                // put souce patient into context
                CurrentContext.Lock(source);

                // do reconciliation
                destPatient.Reconcile(source);

                // delete the source patient
                //- NB: this doesn't work due to some NHibernate issues
                //broker.Delete(source);
            }
        }

        [UpdateOperation]
        public Patient CreatePatientForProfile(PatientProfile profile)
        {
            Patient patient = new Patient();
            patient.AddProfile(profile);

            this.CurrentContext.Lock(patient);

            return patient;
        }

        [UpdateOperation]
        public void UpdatePatientProfile(PatientProfile profile)
        {
            this.CurrentContext.Lock(profile);
            this.CurrentContext.Lock(profile.Patient);   // do we need to do this?
        }

        [UpdateOperation]
        public void UpdateVisit(Visit visit)
        {
            this.CurrentContext.Lock(visit);
            this.CurrentContext.Lock(visit.Patient);
            //IVisitBroker visitBroker = this.CurrentContext.GetBroker<IVisitBroker>();
            //visitBroker.Store(visit);

            //IPatientBroker patientBroker = this.CurrentContext.GetBroker<IPatientBroker>();
            //patientBroker.Store(visit.Patient);
        }

        //[UpdateOperation]
        //public void UpdatePatientVisits(Patient patient)
        //{
        //    this.CurrentContext.Lock(patient);

        //    foreach (Visit visit in patient.Visits)
        //    {
        //        this.CurrentContext.Lock(visit);
        //    }

        //}

        #endregion
    }
}
