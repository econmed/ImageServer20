#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using System.IO;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Static members of this class are safe for use by multiple threads.
    /// </remarks>
    public static class RandomUtils
    {
        // do not use this member directly even within this class - use the static methods instead
        private static readonly Random _random = new Random(Platform.Time.Millisecond);

        private static LoadPatientProfileEditorFormDataResponse _patientEditorFormData;
        private static LoadVisitEditorFormDataResponse _visitEditorFormData;
        private static GetOrderEntryFormDataResponse _orderEntryFormData;
        private static List<DiagnosticServiceSummary> _diagnosticServices;
        private static List<ExternalPractitionerSummary> _practitioners;
        private static bool _refDataCachedInitialized = false;
        private static readonly object _syncLock = new object();



        #region Basic Utilities

        public static int GetRandomInteger(int min, int max)
        {
            // lock is required because Random.Next is not thread-safe
            lock(_random)
            {
                return _random.Next(min, max);
            }
        }

        public static char GetRandomAlphaChar()
        {
            return Convert.ToChar(Convert.ToInt32(GetRandomInteger(0, 25) + 65));
        }

        public static string GenerateRandomIntegerString(int length)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                builder.Append(GetRandomInteger(0, 9).ToString());
            }

            return builder.ToString();
        }

        public static string GenerateRandomString(int length)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                builder.Append(GetRandomAlphaChar());
            }

            return builder.ToString();
        }

        public static TItem ChooseRandom<TItem>(IList<TItem> target)
        {
            if (target.Count == 0)
                return default(TItem);

            if (target.Count == 1)
                return target[0];

            int randomIndex = GetRandomInteger(0, target.Count - 1);
            return target[randomIndex];
        }

        private static string GetRandomNameFromFile(string file)
        {
            List<string> nameList = new List<string>();

            using (TextReader reader = new StringReader(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    nameList.Add(line);
                }
            }

            return ChooseRandom(nameList);
        }

        #endregion

        private static void InitReferenceDataCacheOnce()
        {
            if (!_refDataCachedInitialized)
            {
                lock (_syncLock)
                {
                    if (!_refDataCachedInitialized)
                    {
                        Platform.GetService<IPatientAdminService>(
                            delegate(IPatientAdminService service)
                            {
                                _patientEditorFormData = service.LoadPatientProfileEditorFormData(new LoadPatientProfileEditorFormDataRequest());
                            });

                        Platform.GetService<IVisitAdminService>(
                            delegate(IVisitAdminService service)
                            {
                                _visitEditorFormData = service.LoadVisitEditorFormData(new LoadVisitEditorFormDataRequest());
                            });

                        Platform.GetService<IOrderEntryService>(
                            delegate(IOrderEntryService service)
                            {
                                _orderEntryFormData = service.GetOrderEntryFormData(new GetOrderEntryFormDataRequest());
                            });

                        // cache up to 1000 diagnostic services
                        Platform.GetService<IDiagnosticServiceAdminService>(
                            delegate(IDiagnosticServiceAdminService service)
                            {
                                ListDiagnosticServicesRequest request = new ListDiagnosticServicesRequest();
                                request.Page.FirstRow = 0;
                                request.Page.MaxRows = 1000;
                                _diagnosticServices = service.ListDiagnosticServices(request).DiagnosticServices;
                            });

                        // cache up to 1000 practitioners
                        Platform.GetService<IExternalPractitionerAdminService>(
                            delegate(IExternalPractitionerAdminService service)
                            {
                                ListExternalPractitionersRequest request = new ListExternalPractitionersRequest();
                                request.Page.FirstRow = 0;
                                request.Page.MaxRows = 1000;
                                _practitioners = service.ListExternalPractitioners(request).Practitioners;
                            });

                        _refDataCachedInitialized = true;
                    }
                }
            }
        }

        public static string FormatDateTime(DateTime dateTime, string format)
        {
            if (String.IsNullOrEmpty(format))
                format = "YYYYMMDDmmssxxx";

            string result = format;
            result = result.Replace("YYYY", dateTime.Year.ToString());
            result = result.Replace("MM", dateTime.Month.ToString("00"));
            result = result.Replace("DD", dateTime.Day.ToString("00"));
            result = result.Replace("hh", dateTime.Hour.ToString("00"));
            result = result.Replace("mm", dateTime.Minute.ToString("00"));
            result = result.Replace("ss", dateTime.Second.ToString("00"));
            result = result.Replace("xxx", dateTime.Millisecond.ToString("000"));
            return result.Trim();
        }

        /// <summary>
        /// Creates a new patient with a single profile.
        /// </summary>
        /// <returns></returns>
        public static PatientProfileSummary CreatePatient()
        {
            InitReferenceDataCacheOnce();

            PatientProfileDetail profile = null;

            DateTime now = Platform.Time;

            profile = new PatientProfileDetail();

            profile.Mrn = new CompositeIdentifierDetail(
                GenerateRandomIntegerString(10),
                ChooseRandom(_patientEditorFormData.MrnAssigningAuthorityChoices));

            profile.Healthcard = new HealthcardDetail(
                GenerateRandomIntegerString(10),
                ChooseRandom(_patientEditorFormData.HealthcardAssigningAuthorityChoices),
                "", null);

            profile.DateOfBirth = now;
            profile.Sex = ChooseRandom(_patientEditorFormData.SexChoices);
            profile.PrimaryLanguage = ChooseRandom(_patientEditorFormData.PrimaryLanguageChoices);
            profile.Religion = ChooseRandom(_patientEditorFormData.ReligionChoices);
            profile.DeathIndicator = false;
            profile.TimeOfDeath = null;

            string givenName;
            string familyName = GetRandomNameFromFile(RandomUtilsSettings.Default.FamilyNameDictionary);
            if (profile.Sex.Code == "F")
                givenName = GetRandomNameFromFile(RandomUtilsSettings.Default.FemaleNameDictionary);
            else
                givenName = GetRandomNameFromFile(RandomUtilsSettings.Default.MaleNameDictionary);

            givenName += " Anonymous";
            profile.Name = new PersonNameDetail();
            profile.Name.FamilyName = familyName;
            profile.Name.GivenName = givenName;

            AddPatientResponse addResponse = null;

            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    addResponse = service.AddPatient(new AddPatientRequest(profile));
                });

            return addResponse.PatientProfile;
        }

        /// <summary>
        /// Creates a new visit for the specified patient.
        /// </summary>
        /// <param name="patientRef">Patient for which the visit is created.</param>
        /// <param name="informationAuthority">Information authority to use for the visit number.</param>
        /// <param name="admitOffsetDays">A positive or negative number of days from today.</param>
        /// <returns></returns>
        public static VisitSummary CreateVisit(EntityRef patientRef, EnumValueInfo informationAuthority, int admitOffsetDays)
        {
            InitReferenceDataCacheOnce();

            // Generate an active visit with randomize properties

            DateTime now = Platform.Time;

            VisitDetail visitDetail = new VisitDetail();
            visitDetail.PatientRef = patientRef;
            visitDetail.VisitNumber = new CompositeIdentifierDetail(GenerateRandomIntegerString(10), informationAuthority);
            visitDetail.PatientClass = ChooseRandom(_visitEditorFormData.PatientClassChoices);
            visitDetail.PatientType = ChooseRandom(_visitEditorFormData.PatientTypeChoices);
            visitDetail.AdmissionType = ChooseRandom(_visitEditorFormData.AdmissionTypeChoices);
            visitDetail.Status = CollectionUtils.SelectFirst(_visitEditorFormData.VisitStatusChoices,
                delegate(EnumValueInfo enumValue)
                {
                    return enumValue.Code == "AA";
                });
            visitDetail.AdmitTime = now + TimeSpan.FromDays(admitOffsetDays);
            visitDetail.Facility = ChooseRandom(_visitEditorFormData.FacilityChoices);

            VisitSummary visit = null;
            Platform.GetService<IVisitAdminService>(
                delegate(IVisitAdminService service)
                {
                    AddVisitResponse addVisitResponse = service.AddVisit(new AddVisitRequest(visitDetail));
                    visit = addVisitResponse.Visit;
                });

            return visit;
        }

        /// <summary>
        /// Create a random order on the specified visit.
        /// </summary>
        /// <param name="visit">Visit/patient for which the order is created.</param>
        /// <param name="informationAuthority">Performing facility will be selected to match this information authority.</param>
        /// <param name="schedulingOffsetDays">A positive or negative number of days from today.</param>
        /// <returns></returns>
        public static OrderSummary RandomOrder(VisitSummary visit, EnumValueInfo informationAuthority, int schedulingOffsetDays)
        {
            return RandomOrder(visit, informationAuthority, null, schedulingOffsetDays);
        }

        /// <summary>
        /// Create a random order on the specified visit.
        /// </summary>
        /// <param name="visit">Visit/patient for which the order is created.</param>
        /// <param name="informationAuthority">Performing facility will be selected to match this information authority.</param>
        /// <param name="diagnosticServiceName">Name of the diagnostic service to order.</param>
        /// <param name="schedulingOffsetDays">A positive or negative number of days from today.</param>
        /// <returns></returns>
        public static OrderSummary RandomOrder(VisitSummary visit, EnumValueInfo informationAuthority, string diagnosticServiceName, int schedulingOffsetDays)
        {
            InitReferenceDataCacheOnce();

            DateTime scheduledTime = Platform.Time + TimeSpan.FromDays(schedulingOffsetDays);

            OrderSummary orderSummary = null;
            Platform.GetService<IOrderEntryService>(
                delegate(IOrderEntryService service)
                {
                    DiagnosticServiceSummary diagnosticService;
                    if (String.IsNullOrEmpty(diagnosticServiceName))
                    {
                        diagnosticService = ChooseRandom(_diagnosticServices);                        
                    }
                    else
                    {
                        diagnosticService = CollectionUtils.SelectFirst(
                            _diagnosticServices,
                            delegate(DiagnosticServiceSummary ds)
                            {
                                return ds.Name == diagnosticServiceName;
                            });

                        if (diagnosticService == null)
                            throw new Exception(String.Format("Cannot find diagnostic service with name {0}", diagnosticServiceName));
                    }

                    FacilitySummary performingFacility = CollectionUtils.SelectFirst(_orderEntryFormData.FacilityChoices,
                        delegate(FacilitySummary facility)
                            {
                                return facility.InformationAuthority.Code == informationAuthority.Code;
                            });
                    ExternalPractitionerSummary randomPhysician = ChooseRandom(_practitioners);
                    EnumValueInfo randomPriority = ChooseRandom(_orderEntryFormData.OrderPriorityChoices);

                    OrderRequisition requisition = new OrderRequisition();
                    requisition.Patient = visit.PatientRef;
                    requisition.Visit = visit;
                    requisition.DiagnosticService = diagnosticService;
                    requisition.OrderingPractitioner = randomPhysician;
                    requisition.OrderingFacility = performingFacility;
                    requisition.Priority = randomPriority;
                    requisition.ReasonForStudy = "Randomly generated test order";
                    requisition.SchedulingRequestTime = scheduledTime;

                    LoadDiagnosticServiceBreakdownResponse dsResponse = service.LoadDiagnosticServiceBreakdown(
                        new LoadDiagnosticServiceBreakdownRequest(diagnosticService.DiagnosticServiceRef));

                    requisition.Procedures = new List<ProcedureRequisition>();
                    requisition.Procedures.AddRange(
                       CollectionUtils.Map<ProcedureTypeDetail, ProcedureRequisition>(
                           dsResponse.DiagnosticServiceDetail.ProcedureTypes,
                           delegate(ProcedureTypeDetail rpt)
                           {
                               ProcedureRequisition req = new ProcedureRequisition(rpt.GetSummary(), performingFacility);
                               req.ScheduledTime = scheduledTime;
                               return req;
                           }));
                   
                    requisition.ResultRecipients = new List<ResultRecipientSummary>();
                    requisition.Attachments = new List<OrderAttachmentSummary>();
                    requisition.Notes = new List<OrderNoteDetail>();

                    PlaceOrderResponse response = service.PlaceOrder(new PlaceOrderRequest(requisition));

                    orderSummary = response.Order;
                });

            return orderSummary;
        }
    }
}
