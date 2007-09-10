using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class RegistrationWorklistItem : DataContractBase
    {
        public RegistrationWorklistItem(EntityRef profileRef,
            EntityRef orderRef,
            string mrnID,
            string mrnAssigningAuthority,
            PersonNameDetail name,
            HealthcardDetail healthcard,
            DateTime? dateOfBirth,
            EnumValueInfo sex,
            DateTime? earliestScheduledTime,
            EnumValueInfo orderPriority,
            string patientClass,
            string accessionNumber)
        {
            this.PatientProfileRef = profileRef;
            this.OrderRef = orderRef;
            this.Mrn = new MrnDetail(mrnID, mrnAssigningAuthority);
            this.Name = name;
            this.Healthcard = healthcard;
            this.DateOfBirth = dateOfBirth;
            this.Sex = sex;
            this.EarliestScheduledTime = earliestScheduledTime;
            this.OrderPriority = orderPriority;
            this.PatientClass = patientClass;
            this.AccessionNumber = accessionNumber;
        }

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public MrnDetail Mrn;

        [DataMember]
        public PersonNameDetail Name;

        [DataMember]
        public HealthcardDetail Healthcard;

        [DataMember]
        public DateTime? DateOfBirth;

        [DataMember]
        public EnumValueInfo Sex;

        [DataMember]
        public DateTime? EarliestScheduledTime;

        [DataMember]
        public EnumValueInfo OrderPriority;

        [DataMember]
        public string PatientClass;

        [DataMember]
        public string AccessionNumber;

        public override bool Equals(object obj)
        {
            RegistrationWorklistItem that = obj as RegistrationWorklistItem;
            if (that != null)
                return this.PatientProfileRef.Equals(that.PatientProfileRef)
                    && (this.OrderRef == null ? true : this.OrderRef.Equals(that.OrderRef));

            return false;
        }

        public override int GetHashCode()
        {
            return this.PatientProfileRef.GetHashCode()
                + (this.OrderRef == null ? 0 : this.OrderRef.GetHashCode());
        }
    }
}
