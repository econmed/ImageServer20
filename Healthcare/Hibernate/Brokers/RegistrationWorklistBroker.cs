using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using System.Collections;
using NHibernate;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class RegistrationWorklistBroker : Broker, IRegistrationWorklistBroker
    {
        public IList<WorklistItem> GetWorklist(string worklistClassName)
        {
            string defaultHqlQuery = "select distinct pp from ModalityProcedureStep mps" +
                            " join mps.Type spst" +
                            " join mps.Modality m" +
                            " join mps.RequestedProcedure rp" +
                            " join rp.Type rpt" +
                            " join rp.Order o" +
                            " join o.DiagnosticService ds" +
                            " join o.Visit v" +
                            " join o.Patient p" +
                            " join p.Profiles pp" +
                            " where mps.State = :mpsState";
            //" and mps.Scheduling.StartTime between :mpsSchedulingStartTimeBegin and :mpsSchedulingStartTimeEnd";

            string hqlQuery = String.Concat(defaultHqlQuery, GetSubQuery(worklistClassName));
            IQuery query = this.Context.CreateHibernateQuery(hqlQuery);
            SetNamedParameters(query, worklistClassName);

            List<WorklistItem> results = new List<WorklistItem>();
            foreach (object tuple in query.List())
            {
                WorklistItem item = (WorklistItem)Activator.CreateInstance(typeof(WorklistItem), tuple);
                item.WorklistClassName = worklistClassName;
                results.Add(item);
            }
            return results;
        }

        private void SetNamedParameters(IQuery query, string worklistClassName)
        {
            //query.SetParameter("mpsSchedulingStartTimeBegin", Platform.Time.Date.ToString());
            //query.SetParameter("mpsSchedulingStartTimeEnd", Platform.Time.Date.AddDays(1).ToString());

            switch (worklistClassName)
            {
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Scheduled":
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+CheckIn":
                    query.SetParameter("mpsState", "SC");
                    //query.SetParameter("cpsSchedulingStartTimeBegin", Platform.Time.Date.ToString());
                    //query.SetParameter("cpsSchedulingStartTimeEnd", Platform.Time.Date.AddDays(1).ToString());
                    break;
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+InProgress":
                    query.SetParameter("mpsState", "IP");
                    break;
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Completed":
                    query.SetParameter("mpsState", "CM");
                    break;
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Cancelled":
                    query.SetParameter("mpsState", "DC");
                    break;
                default:
                    break;
            }
        }

        private string GetSubQuery(string worklistClassName)
        {
            string subQuery = null;
            switch (worklistClassName)
            {
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Scheduled":
                    subQuery = " and rp not in (select rp from CheckInProcedureStep cps join cps.RequestedProcedure rp" +
                                //" where (cps.Scheduling.StartTime between :cpsSchedulingStartTimeBegin and :cpsSchedulingStartTimeEnd)";
                                ")";
                    break;
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+CheckIn":
                    subQuery = " and rp in (select rp from CheckInProcedureStep cps join cps.RequestedProcedure rp" +
                                //" where (cps.Scheduling.StartTime between :cpsSchedulingStartTimeBegin and :cpsSchedulingStartTimeEnd)";
                                ")";
                    break;
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+InProgress":
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Completed":
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Cancelled":
                default:
                    break;
            }

            return subQuery;
        }

        public IList<RequestedProcedure> GetRequestedProcedureForPatient(Patient patient, string worklistClassName)
        {
            List<RequestedProcedure> results = new List<RequestedProcedure>();
            try
            {
                string hqlQuery = "select distinct rp from ModalityProcedureStep mps" +
                                " join mps.Type spst" +
                                " join mps.Modality m" +
                                " join mps.RequestedProcedure rp" +
                                " join rp.Type rpt" +
                                " join rp.Order o" +
                                " join o.DiagnosticService ds" +
                                " join o.Visit v" +
                                " join o.Patient p" +
                                " where mps.State = :mpsState and p = :patient";
                //" where (cps.Scheduling.StartTime between :cpsSchedulingStartTimeBegin and :cpsSchedulingStartTimeEnd)";

                hqlQuery = String.Concat(hqlQuery, GetSubQuery(worklistClassName));
                IQuery query = this.Context.CreateHibernateQuery(hqlQuery);
                SetNamedParameters(query, worklistClassName);
                query.SetParameter("patient", patient);

                foreach (object tuple in query.List())
                {
                    RequestedProcedure item = (RequestedProcedure)tuple;
                    results.Add(item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return results;
        }

    }
}
