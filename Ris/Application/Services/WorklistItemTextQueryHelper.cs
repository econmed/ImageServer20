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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using System;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{

    public class WorklistItemTextQueryHelper<TDomainItem, TSummary>
        : TextQueryHelper<TDomainItem, WorklistItemSearchCriteria, TSummary>
        where TDomainItem : WorklistItemBase
        where TSummary : DataContractBase
    {
    	private readonly Type _procedureStepClass;
    	private readonly IWorklistItemBroker<TDomainItem> _broker;
    	private readonly WorklistItemTextQueryOptions _options;

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorklistItemTextQueryHelper(
			IWorklistItemBroker<TDomainItem> broker,
			Converter<TDomainItem, TSummary> summaryAssembler,
			Type procedureStepClass,
			WorklistItemTextQueryOptions options)
			: base(null, summaryAssembler, null, null)
		{
			_broker = broker;
			_procedureStepClass = procedureStepClass;
			_options = options;
		}

		#region Overrides

		protected override WorklistItemSearchCriteria[] BuildCriteria(string query)
        {
			List<WorklistItemSearchCriteria> criteria = new List<WorklistItemSearchCriteria>();

			if ((_options & WorklistItemTextQueryOptions.PatientOrder) == WorklistItemTextQueryOptions.PatientOrder)
			{
				criteria.AddRange(BuildCriteriaForPatientOrderSearch(query));
			}

			if ((_options & WorklistItemTextQueryOptions.ProcedureStepStaff) == WorklistItemTextQueryOptions.ProcedureStepStaff)
			{
				criteria.AddRange(BuildCriteriaForStaffSearch(query));
			}

			// add constraint for downtime vs live procedures
			bool downtimeRecoveryMode = (_options & WorklistItemTextQueryOptions.DowntimeRecovery) ==
										WorklistItemTextQueryOptions.DowntimeRecovery;
			criteria.ForEach(delegate(WorklistItemSearchCriteria c) { c.Procedure.DowntimeRecoveryMode.EqualTo(downtimeRecoveryMode); });

        	return criteria.ToArray();
		}

		protected override bool TestSpecificity(WorklistItemSearchCriteria[] where, int threshold)
		{
			int count;
			return _broker.EstimateSearchResultsCount(where, threshold, IncludeDegenerateItems, out count);
		}

		protected override IList<TDomainItem> DoQuery(WorklistItemSearchCriteria[] where, SearchResultPage page)
		{
			return _broker.GetSearchResults(where, IncludeDegenerateItems);
		}

		#endregion

		#region Helpers

    	private bool IncludeDegenerateItems
    	{
    		get
    		{
				// generally, if the search query is being used on patients/orders, then it makes sense to include
				// degenerate items
				// conversely, if this flag is not present, then including degenerate items would mean result in an open query
				// on the entire database which would obviously not be desirable
    			return (_options & WorklistItemTextQueryOptions.PatientOrder) == WorklistItemTextQueryOptions.PatientOrder;
    		}
    	}

		private List<WorklistItemSearchCriteria> BuildCriteriaForPatientOrderSearch(string query)
		{
			// this will hold all criteria
			List<WorklistItemSearchCriteria> criteria = new List<WorklistItemSearchCriteria>();

			// build criteria against names
			PersonName[] names = ParsePersonNames(query);
			criteria.AddRange(CollectionUtils.Map<PersonName, WorklistItemSearchCriteria>(names,
				delegate(PersonName n)
				{
					WorklistItemSearchCriteria sc = new WorklistItemSearchCriteria(_procedureStepClass);
					sc.PatientProfile.Name.FamilyName.StartsWith(n.FamilyName);
					if (n.GivenName != null)
						sc.PatientProfile.Name.GivenName.StartsWith(n.GivenName);
					return sc;
				}));

			// build criteria against Mrn identifiers
			string[] ids = ParseIdentifiers(query);
			criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
				delegate(string word)
				{
					WorklistItemSearchCriteria c = new WorklistItemSearchCriteria(_procedureStepClass);
					c.PatientProfile.Mrn.Id.StartsWith(word);
					return c;
				}));

			// build criteria against Healthcard identifiers
			criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
				delegate(string word)
				{
					WorklistItemSearchCriteria c = new WorklistItemSearchCriteria(_procedureStepClass);
					c.PatientProfile.Healthcard.Id.StartsWith(word);
					return c;
				}));

			// build criteria against Accession Number
			criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
				delegate(string word)
				{
					WorklistItemSearchCriteria c = new WorklistItemSearchCriteria(_procedureStepClass);
					c.Order.AccessionNumber.StartsWith(word);
					return c;
				}));

			return criteria;
		}

		private List<WorklistItemSearchCriteria> BuildCriteriaForStaffSearch(string query)
		{
			// this will hold all criteria
			List<WorklistItemSearchCriteria> criteria = new List<WorklistItemSearchCriteria>();

			// build criteria against names
			PersonName[] names = ParsePersonNames(query);

			// scheduled performer
			criteria.AddRange(CollectionUtils.Map<PersonName, WorklistItemSearchCriteria>(names,
				delegate(PersonName n)
				{
					WorklistItemSearchCriteria sc = new WorklistItemSearchCriteria(_procedureStepClass);

					PersonNameSearchCriteria scheduledPerformerNameCriteria = sc.ProcedureStep.Scheduling.Performer.Staff.Name;
					scheduledPerformerNameCriteria.FamilyName.StartsWith(n.FamilyName);
					if (n.GivenName != null)
						scheduledPerformerNameCriteria.GivenName.StartsWith(n.GivenName);
					return sc;
				}));

			// actual performer
			criteria.AddRange(CollectionUtils.Map<PersonName, WorklistItemSearchCriteria>(names,
				delegate(PersonName n)
				{
					WorklistItemSearchCriteria sc = new WorklistItemSearchCriteria(_procedureStepClass);

					PersonNameSearchCriteria performerNameCriteria = sc.ProcedureStep.Performer.Staff.Name;
					performerNameCriteria.FamilyName.StartsWith(n.FamilyName);
					if (n.GivenName != null)
						performerNameCriteria.GivenName.StartsWith(n.GivenName);
					return sc;
				}));

			// build criteria against Staff ID identifiers
			string[] ids = ParseIdentifiers(query);

			// scheduled performer
			criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
				delegate(string id)
				{
					WorklistItemSearchCriteria sc = new WorklistItemSearchCriteria(_procedureStepClass);
					sc.ProcedureStep.Scheduling.Performer.Staff.Id.StartsWith(id);
					return sc;
				}));

			// actual performer
			criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
				delegate(string id)
				{
					WorklistItemSearchCriteria sc = new WorklistItemSearchCriteria(_procedureStepClass);
					sc.ProcedureStep.Performer.Staff.Id.StartsWith(id);
					return sc;
				}));

			return criteria;
		}

		#endregion
	}
}