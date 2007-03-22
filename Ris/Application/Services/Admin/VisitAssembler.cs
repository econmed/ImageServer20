using System;
using System.Collections.Generic;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class VisitAssembler
    {
        public VisitSummary CreateVisitSummary(Visit visit, IPersistenceContext context)
        {
            VisitSummary summary = new VisitSummary();

            summary.entityRef = visit.GetRef;
            summary.Patient = visit.Patient.GetRef;

            summary.VisitNumberAssigningAuthority = visit.VisitNumber.AssigningAuthority;
            summary.VisitNumberId = visit.VisitNumber.Id;

            summary.AdmissionType = context.GetBroker<IAdmissionTypeEnumBroker>().Load()[visit.AdmissionType];
            summary.PatientClass = context.GetBroker<IPatientClassEnumBroker>().Load()[visit.PatientClass];
            summary.PatientType = context.GetBroker<IPatientTypeEnumBroker>().Load()[visit.PatientType];
            summary.Status = context.GetBroker<IVisitStatusEnumBroker>().Load()[visit.VisitStatus];

            summary.AdmitDateTime = visit.AdmitDateTime;
            summary.DischargeDateTime = visit.DischargeDateTime;

            return summary;
        }

        public VisitDetail CreateVisitDetail(Visit visit, IPersistenceContext context)
        {
            VisitDetail detail = new VisitDetail();

            detail.VisitNumberAssigningAuthority = visit.VisitNumber.AssigningAuthority;
            detail.VisitNumberId = visit.VisitNumber.Id;

            AdmissionTypeEnum admissionType = context.GetBroker<IAdmissionTypeEnumBroker>().Load()[visit.AdmissionType];
            detail.AdmissionType = new EnumValueInfo(admissionType.Code, admissionType.Value);

            PatientClassEnum patientClass = context.GetBroker<IPatientClassEnumBroker>().Load()[visit.PatientClass];
            detail.PatientClass = new EnumValueInfo(patientClass.Code, patientClass.Value);

            PatientTypeEnum patientType = context.GetBroker<IPatientTypeEnumBroker>().Load()[visit.PatientType];
            detail.PatientType = new EnumValueInfo(patientType.Code, patientType.Value);

            VisitStatusEnum status = context.GetBroker<IVisitStatusEnumBroker>().Load()[visit.VisitStatus];
            detail.Status = new EnumValueInfo(status.Code, status.Value);


            detail.AdmitDateTime = visit.AdmitDateTime;
            detail.DischargeDateTime = visit.DischargeDateTime;

            detail.DischargeDisposition = visit.DischargeDisposition;

            detail.Facility = new FacilityAssembler().CreateFacilitySummary(visit.Facility);
            
            detail.Locations = new List<VisitLocationDetail>();
            foreach (VisitLocation vl in visit.Locations)
            {
                detail.Locations.Add(CreateVisitLocationDetail(vl, context));
            }

            detail.Practitioners = new List<VisitPractitionerDetail>();
            foreach (VisitPractitioner vp in visit.Practitioners)
            {
                detail.Practitioners.Add(CreateVisitPractitionerDetail(vp, context));
            }

            detail.AmbulatoryStatuses = new List<EnumValueInfo>();
            AmbulatoryStatusEnumTable ambulatoryStatuses = context.GetBroker<IAmbulatoryStatusEnumBroker>().Load();
            foreach (AmbulatoryStatus status in visit.AmbulatoryStatuses)
	        {
		        detail.AmbulatoryStatuses.Add(new EnumValueInfo(ambulatoryStatuses[status].Code, ambulatoryStatuses[status].Value));
	        }   
         
            detail.PreadmitNumber = visit.PreadmitNumber;
            detail.VipIndicator = visit.VisitNumber;

            return detail;
        }

        public void UpdateVisit(Visit visit, VisitDetail detail, IPersistenceContext context)
        {
            visit.VisitNumber.Id = detail.VisitNumberId;
            visit.VisitNumber.AssigningAuthority = detail.VisitNumberAssigningAuthority;

            visit.AdmissionType = (AdmissionType)Enum.Parse(typeof(AdmissionType), detail.AdmissionType.Code);
            visit.PatientClass = (PatientClass)Enum.Parse(typeof(PatientClass), detail.PatientClass.Code);
            visit.PatientType = (PatientType)Enum.Parse(typeof(PatientType), detail.PatientType.Code);
            visit.VisitStatus = (VisitStatus)Enum.Parse(typeof(VisitStatus), detail.Status.Code);

            visit.AdmitDateTime = detail.AdmitDateTime;
            visit.DischargeDateTime = detail.DischargeDateTime;
            visit.DischargeDisposition = detail.DischargeDisposition;

            visit.Facility = (Facility)context.Load(detail.Facility.FacilityRef, EntityLoadFlags.Proxy);

            visit.Locations.Clear();
            foreach (VisitLocationDetail vlDetail in detail.Locations)
            {
                visit.Locations.Add(new VisitLocation(
                    (Location)context.Load(vlDetail.Location.LocationRef, EntityLoadFlags.Proxy),
                    (VisitLocationRole)Enum.Parse(typeof(VisitLocationRole), vlDetail.Role.Code),
                    vlDetail.StartTime,
                    vlDetail.EndTime));
            }

            visit.Practitioners.Clear();
            foreach (VisitPractitionerDetail vpDetail in detail.Practitioners)
            {
                visit.Practitioners.Add(new VisitPractitioner(
                    (Practitioner)context.Load(vpDetail.Practitioner.StaffRef, EntityLoadFlags.Proxy),
                    (VisitPractitionerRole)Enum.Parse(typeof(VisitPractitionerRole), vpDetail.Role.Code),
                    vpDetail.StartTime,
                    vpDetail.EndTime));
            }

            visit.AmbulatoryStatuses.Clear();
            foreach (EnumValueInfo ambulatoryStatus in detail.AmbulatoryStatuses)
            {
                visit.AmbulatoryStatuses.Add((AmbulatoryStatus)Enum.Parse(typeof(AmbulatoryStatus), ambulatoryStatus.Code));   
            }
        }

        private VisitLocationDetail CreateVisitLocationDetail(VisitLocation vl, IPersistenceContext context)
        {
            VisitLocationDetail detail = new VisitLocationDetail();

            detail.Location = new LocationAssembler().CreateLocationSummary(vl.Location);

            VisitPractitionerRoleEnum role = context.GetBroker<IVisitPractitionerRoleEnumBroker>().Load()[vl.Role];
            detail.Role = new EnumValueInfo(role.Code, role.Value);

            detail.StartTime = vl.StartTime;
            detail.EndTime = vl.EndTime;

            return detail;
        }

        private VisitPractitionerDetail CreateVisitPractitionerDetail(VisitPractitioner vp, IPersistenceContext context)
        {
            VisitPractitionerDetail detail = new VisitPractitionerDetail();
            
            detail.Practitioner = new PractitionerAssembler().CreatePractitionerSummary(vp.Practitioner);
            
            VisitPractitionerRoleEnum role = context.GetBroker<IVisitPractitionerRoleEnumBroker>().Load()[vp.Role];
            detail.Role = new EnumValueInfo(role.Code, role.Value);

            detail.StartTime = vp.StartTime;
            detail.EndTime = vp.EndTime;

            return detail;
        }
    }
}
