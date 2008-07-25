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
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.ExternalPractitionerAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IExternalPractitionerAdminService))]
    public class ExternalPractitionerAdminService : ApplicationServiceBase, IExternalPractitionerAdminService
    {
        #region IExternalPractitionerAdminService Members

        [ReadOperation]
        // note: this operation is not protected with ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin
        // because it is used in non-admin situations - perhaps we need to create a separate operation???
        public ListExternalPractitionersResponse ListExternalPractitioners(ListExternalPractitionersRequest request)
        {
            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();

            ExternalPractitionerSearchCriteria criteria = new ExternalPractitionerSearchCriteria();
			criteria.Name.FamilyName.SortAsc(0);

            if (!string.IsNullOrEmpty(request.FirstName))
                criteria.Name.GivenName.StartsWith(request.FirstName);
            if (!string.IsNullOrEmpty(request.LastName))
                criteria.Name.FamilyName.StartsWith(request.LastName);
			if (!request.IncludeDeactivated)
				criteria.Deactivated.EqualTo(false);

			return new ListExternalPractitionersResponse(
                CollectionUtils.Map<ExternalPractitioner, ExternalPractitionerSummary, List<ExternalPractitionerSummary>>(
                    PersistenceContext.GetBroker<IExternalPractitionerBroker>().Find(criteria, request.Page),
                    delegate(ExternalPractitioner s)
                    {
                        return assembler.CreateExternalPractitionerSummary(s, PersistenceContext);
                    }));
        }

        [ReadOperation]
        //[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin)]
        public LoadExternalPractitionerForEditResponse LoadExternalPractitionerForEdit(LoadExternalPractitionerForEditRequest request)
        {
            // note that the version of the ExternalPractitionerRef is intentionally ignored here (default behaviour of ReadOperation)
            ExternalPractitioner s = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerRef);
            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();

            return new LoadExternalPractitionerForEditResponse(assembler.CreateExternalPractitionerDetail(s, this.PersistenceContext));
        }

        [ReadOperation]
        public LoadExternalPractitionerEditorFormDataResponse LoadExternalPractitionerEditorFormData(LoadExternalPractitionerEditorFormDataRequest request)
        {
            return new LoadExternalPractitionerEditorFormDataResponse(
                EnumUtils.GetEnumValueList<AddressTypeEnum>(PersistenceContext),
                (new SimplifiedPhoneTypeAssembler()).GetPractitionerPhoneTypeChoices(),
                EnumUtils.GetEnumValueList<ResultCommunicationModeEnum>(PersistenceContext));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ExternalPractitioner)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Create)]
		public AddExternalPractitionerResponse AddExternalPractitioner(AddExternalPractitionerRequest request)
        {
            ExternalPractitioner prac = new ExternalPractitioner();

            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();
            assembler.UpdateExternalPractitioner(request.PractitionerDetail, prac, PersistenceContext);

            PersistenceContext.Lock(prac, DirtyState.New);

            // ensure the new prac is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddExternalPractitionerResponse(assembler.CreateExternalPractitionerSummary(prac, PersistenceContext));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ExternalPractitioner)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Update)]
		public UpdateExternalPractitionerResponse UpdateExternalPractitioner(UpdateExternalPractitionerRequest request)
        {
            ExternalPractitioner prac = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerDetail.PractitionerRef, EntityLoadFlags.CheckVersion);

            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();
            assembler.UpdateExternalPractitioner(request.PractitionerDetail, prac, PersistenceContext);

            return new UpdateExternalPractitionerResponse(assembler.CreateExternalPractitionerSummary(prac, PersistenceContext));
        }

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ExternalPractitioner)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Merge)]
		public DeleteExternalPractitionerResponse DeleteExternalPractitioner(DeleteExternalPractitionerRequest request)
		{
			try
			{
				IExternalPractitionerBroker broker = PersistenceContext.GetBroker<IExternalPractitionerBroker>();
				ExternalPractitioner practitioner = broker.Load(request.PractitionerRef, EntityLoadFlags.Proxy);
				broker.Delete(practitioner);
				PersistenceContext.SynchState();
				return new DeleteExternalPractitionerResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(ExternalPractitioner))));
			}
		}

        [ReadOperation]
		public TextQueryResponse<ExternalPractitionerSummary> TextQuery(TextQueryRequest request)
        {
            IExternalPractitionerBroker broker = PersistenceContext.GetBroker<IExternalPractitionerBroker>();
            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();

            TextQueryHelper<ExternalPractitioner, ExternalPractitionerSearchCriteria, ExternalPractitionerSummary> helper
                = new TextQueryHelper<ExternalPractitioner, ExternalPractitionerSearchCriteria, ExternalPractitionerSummary>(
                    delegate(string rawQuery)
                    {
                        List<ExternalPractitionerSearchCriteria> criteria = new List<ExternalPractitionerSearchCriteria>();

                        // build criteria against names
                        PersonName[] names = TextQueryHelper.ParsePersonNames(rawQuery);
                        criteria.AddRange(CollectionUtils.Map<PersonName, ExternalPractitionerSearchCriteria>(names,
                            delegate(PersonName n)
                            {
                                ExternalPractitionerSearchCriteria sc = new ExternalPractitionerSearchCriteria();
                                sc.Name.FamilyName.StartsWith(n.FamilyName);
                                if (n.GivenName != null)
                                    sc.Name.GivenName.StartsWith(n.GivenName);
                                return sc;
                            }));

                        // build criteria against identifiers
                        string[] ids = TextQueryHelper.ParseIdentifiers(rawQuery);
                        criteria.AddRange(CollectionUtils.Map<string, ExternalPractitionerSearchCriteria>(ids,
                                     delegate(string word)
                                     {
                                         ExternalPractitionerSearchCriteria c = new ExternalPractitionerSearchCriteria();
                                         c.LicenseNumber.StartsWith(word);
                                         return c;
                                     }));

                        return criteria.ToArray();
                    },
                    delegate(ExternalPractitioner prac)
                    {
                        return assembler.CreateExternalPractitionerSummary(prac, PersistenceContext);
                    },
                    delegate(ExternalPractitionerSearchCriteria[] criteria, int threshold)
                    {
                    	return broker.Count(criteria) <= threshold;
                    },
                    delegate(ExternalPractitionerSearchCriteria[] criteria, SearchResultPage page)
                    {
                        return broker.Find(criteria, page);
                    });

            return helper.Query(request);
        }

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Merge)]
		public MergeDuplicatePractitionerResponse MergeDuplicatePractitioner(MergeDuplicatePractitionerRequest request)
		{
			IExternalPractitionerBroker broker = PersistenceContext.GetBroker<IExternalPractitionerBroker>();
			ExternalPractitioner duplicate = broker.Load(request.Duplicate.PractitionerRef, EntityLoadFlags.Proxy);
			ExternalPractitioner original = broker.Load(request.Original.PractitionerRef, EntityLoadFlags.Proxy);

			// Change reference of ordering practitioner to the new practitioner
			CollectionUtils.ForEach(broker.GetRelatedOrders(duplicate), 
				delegate(Order o) { o.OrderingPractitioner = original; });

			// Change reference of contact points to the new practitioner
			CollectionUtils.ForEach(duplicate.ContactPoints,
				delegate(ExternalPractitionerContactPoint cp)
					{
						cp.IsDefaultContactPoint = false;
						cp.Practitioner = original;
					});
			original.ContactPoints.AddAll(duplicate.ContactPoints);

			// Change reference of visit practitioner to the new practitioner
			CollectionUtils.ForEach(broker.GetRelatedVisits(duplicate),
				delegate(Visit v)
					{
						CollectionUtils.ForEach(v.Practitioners,
							delegate(VisitPractitioner vp)
								{
									if (vp.Practitioner == duplicate)
										vp.Practitioner = original;
								});
					});

			ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();
			return new MergeDuplicatePractitionerResponse(assembler.CreateExternalPractitionerSummary(original, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ExternalPractitioner)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Merge)]
		public MergeDuplicateContactPointResponse MergeDuplicateContactPoint(MergeDuplicateContactPointRequest request)
		{
			IExternalPractitionerContactPointBroker broker = PersistenceContext.GetBroker<IExternalPractitionerContactPointBroker>();
			ExternalPractitionerContactPoint duplicate = broker.Load(request.Duplicate.ContactPointRef, EntityLoadFlags.Proxy);
			ExternalPractitionerContactPoint original = broker.Load(request.Original.ContactPointRef, EntityLoadFlags.Proxy);

			// Change reference of result recipient to the new contact point
			CollectionUtils.ForEach(broker.GetRelatedOrders(duplicate),
				delegate(Order o)
					{
						CollectionUtils.ForEach(o.ResultRecipients,
							delegate(ResultRecipient rr)
								{
									if (rr.PractitionerContactPoint == duplicate)
										rr.PractitionerContactPoint = original;
								});
					});

			// copy all addresses/emails/telephones to the new contact point
			CollectionUtils.ForEach(duplicate.Addresses, 
				delegate(Address a) { original.Addresses.Add((Address)a.Clone());});

			CollectionUtils.ForEach(duplicate.EmailAddresses,
				delegate(EmailAddress e) { original.EmailAddresses.Add((EmailAddress)e.Clone()); });

			CollectionUtils.ForEach(duplicate.TelephoneNumbers,
				delegate(TelephoneNumber p) { original.TelephoneNumbers.Add((TelephoneNumber)p.Clone()); });

			PersistenceContext.SynchState();

			// remove the duplicate contact point
			ExternalPractitioner practitioner = original.Practitioner;
			practitioner.ContactPoints.Remove(duplicate);
			
			ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();
			return new MergeDuplicateContactPointResponse(assembler.CreateExternalPractitionerContactPointSummary(original));
		}

		[ReadOperation]
		public LoadMergeDuplicatePractitionerFormDataResponse LoadMergeDuplicatePractitionerFormData(LoadMergeDuplicatePractitionerFormDataRequest request)
		{
			IExternalPractitionerBroker broker = PersistenceContext.GetBroker<IExternalPractitionerBroker>();
			ExternalPractitioner practitioner = broker.Load(request.Practitioner.PractitionerRef, EntityLoadFlags.Proxy);

			OrderAssembler orderAssembler = new OrderAssembler();
			List<OrderSummary> affectedOrders = CollectionUtils.Map<Order, OrderSummary>(
				broker.GetRelatedOrders(practitioner),
				delegate(Order o)
					{
						return orderAssembler.CreateOrderSummary(o, this.PersistenceContext);
					});

			VisitAssembler visitAssembler = new VisitAssembler();
			List<VisitSummary> affectedVisits = CollectionUtils.Map<Visit, VisitSummary>(
				broker.GetRelatedVisits(practitioner),
				delegate(Visit v)
					{
						return visitAssembler.CreateVisitSummary(v, this.PersistenceContext);
					});

			return new LoadMergeDuplicatePractitionerFormDataResponse(affectedOrders, affectedVisits);
		}

		[ReadOperation]
		public LoadMergeDuplicateContactPointFormDataResponse LoadMergeDuplicateContactPointFormData(LoadMergeDuplicateContactPointFormDataRequest request)
		{
			IExternalPractitionerContactPointBroker broker = PersistenceContext.GetBroker<IExternalPractitionerContactPointBroker>();
			ExternalPractitionerContactPoint contactPoint = broker.Load(request.ContactPoint.ContactPointRef, EntityLoadFlags.Proxy);

			OrderAssembler orderAssembler = new OrderAssembler();
			List<OrderSummary> affectedOrders = CollectionUtils.Map<Order, OrderSummary>(
				broker.GetRelatedOrders(contactPoint),
				delegate(Order o)
				{
					return orderAssembler.CreateOrderSummary(o, this.PersistenceContext);
				});

			return new LoadMergeDuplicateContactPointFormDataResponse(affectedOrders);
		}

		#endregion

	}
}
