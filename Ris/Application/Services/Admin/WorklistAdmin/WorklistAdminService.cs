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
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
    [ServiceImplementsContract(typeof(IWorklistAdminService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class WorklistAdminService : ApplicationServiceBase, IWorklistAdminService
    {
        #region IWorklistAdminService Members

        [ReadOperation]
        public ListWorklistCategoriesResponse ListWorklistCategories(ListWorklistCategoriesRequest request)
        {
            List<string> categories = CollectionUtils.Map<Type, string>(
                WorklistFactory.Instance.ListWorklistClasses(true),
                delegate(Type t) { return Worklist.GetCategory(t); });

            // in case some worklist classes did not have assigned categories
            CollectionUtils.Remove(categories, delegate (string s) { return s == null;} );

            return new ListWorklistCategoriesResponse(CollectionUtils.Unique(categories));
        }

        [ReadOperation]
        public ListWorklistClassesResponse ListWorklistClasses(ListWorklistClassesRequest request)
        {
            Platform.CheckForNullReference(request, "request");

            List<Type> worklistClasses =
                ListClassesHelper(request.ClassNames, request.Categories, request.IncludeStatic);

            WorklistAdminAssembler assembler = new WorklistAdminAssembler();
            return new ListWorklistClassesResponse(
                CollectionUtils.Map<Type, WorklistClassSummary>(worklistClasses,
                    delegate(Type worklistClass)
                    {
                        return assembler.CreateClassSummary(worklistClass);
                    }));
        }

        [ReadOperation]
        public ListWorklistsResponse ListWorklists(ListWorklistsRequest request)
        {
            Platform.CheckForNullReference(request, "request");

            List<Type> worklistClasses =
                ListClassesHelper(request.ClassNames, request.Categories, request.IncludeStatic);
            
            // grab the persistent worklists
            IWorklistBroker broker = PersistenceContext.GetBroker<IWorklistBroker>();
            List<string> persistentClassNames = CollectionUtils.Select(worklistClasses,
                delegate(Type t) { return !Worklist.GetIsStatic(t); })
                .ConvertAll<string>(delegate(Type t) { return Worklist.GetClassName(t); });
            IList<Worklist> worklists = broker.FindWorklists(persistentClassNames, request.Page);

            // optionally include the static ones
            if(request.IncludeStatic)
            {
                foreach (Type worklistClass in worklistClasses)
                {
                    if (Worklist.GetIsStatic(worklistClass))
                        worklists.Add(WorklistFactory.Instance.CreateWorklist(worklistClass));
                }
            }

            WorklistAdminAssembler adminAssembler = new WorklistAdminAssembler();
            return new ListWorklistsResponse(
                CollectionUtils.Map<Worklist, WorklistAdminSummary, List<WorklistAdminSummary>>(
                worklists,
                delegate(Worklist worklist)
                {
                    return adminAssembler.GetWorklistSummary(worklist, this.PersistenceContext);
                }));
        }

        [ReadOperation]
        public ListProcedureTypeGroupsResponse ListProcedureTypeGroups(ListProcedureTypeGroupsRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.ProcedureTypeGroupClass, "request.ProcedureTypeGroupClass");

            Type procedureTypeGroupClass = ProcedureTypeGroup.GetSubClass(request.ProcedureTypeGroupClass, PersistenceContext);
            if (procedureTypeGroupClass == null)
                throw new ArgumentException("Invalid ProcedureTypeGroupClass name");

            ListProcedureTypeGroupsResponse response =
                new ListProcedureTypeGroupsResponse();

            ProcedureTypeGroupAssembler assembler = new ProcedureTypeGroupAssembler();
            response.ProcedureTypeGroups =
                CollectionUtils.Map<ProcedureTypeGroup, ProcedureTypeGroupSummary>(
                    PersistenceContext.GetBroker<IProcedureTypeGroupBroker>().Find(new ProcedureTypeGroupSearchCriteria(), procedureTypeGroupClass),
                    delegate(ProcedureTypeGroup group)
                    {
                        return assembler.GetProcedureTypeGroupSummary(group, PersistenceContext);
                    });

            return response;
        }

        [ReadOperation]
        public GetWorklistEditFormDataResponse GetWorklistEditFormData(GetWorklistEditFormDataRequest request)
        {
            GetWorklistEditFormDataResponse response = new GetWorklistEditFormDataResponse();

            WorklistAdminAssembler assembler = new WorklistAdminAssembler();
            response.WorklistClasses = CollectionUtils.Map<Type, WorklistClassSummary>(
                WorklistFactory.Instance.ListWorklistClasses(false),
                delegate(Type worklistClass)
                {
                    return assembler.CreateClassSummary(worklistClass);
                });

            StaffAssembler staffAssembler = new StaffAssembler();
            response.StaffChoices = CollectionUtils.Map<Staff, StaffSummary>(
                this.PersistenceContext.GetBroker<IStaffBroker>().FindAll(),
                delegate(Staff item)
                {
                    return staffAssembler.CreateStaffSummary(item, PersistenceContext);
                });

            StaffGroupAssembler staffGroupAssembler = new StaffGroupAssembler();
            response.StaffGroupChoices = CollectionUtils.Map<StaffGroup, StaffGroupSummary>(
                this.PersistenceContext.GetBroker<IStaffGroupBroker>().FindAll(),
                delegate(StaffGroup item)
                {
                    return staffGroupAssembler.CreateSummary(item);
                });

            FacilityAssembler facilityAssembler = new FacilityAssembler();
            response.FacilityChoices = CollectionUtils.Map<Facility, FacilitySummary>(
                this.PersistenceContext.GetBroker<IFacilityBroker>().FindAll(),
                delegate(Facility f)
                {
                    return facilityAssembler.CreateFacilitySummary(f);
                });

			LocationAssembler locationAssembler = new LocationAssembler();
			response.PatientLocationChoices = CollectionUtils.Map<Location, LocationSummary>(
				this.PersistenceContext.GetBroker<ILocationBroker>().FindAll(),
				delegate(Location l)
				{
					return locationAssembler.CreateLocationSummary(l);
				});

			response.OrderPriorityChoices = EnumUtils.GetEnumValueList<OrderPriorityEnum>(PersistenceContext);
            response.PatientClassChoices = EnumUtils.GetEnumValueList<PatientClassEnum>(PersistenceContext);

            return response;
        }



        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Worklist)]
        public LoadWorklistForEditResponse LoadWorklistForEdit(LoadWorklistForEditRequest request)
        {
            Worklist worklist = PersistenceContext.Load<Worklist>(request.EntityRef);
            WorklistAdminAssembler adminAssembler = new WorklistAdminAssembler();
            WorklistAdminDetail adminDetail = adminAssembler.GetWorklistDetail(worklist, this.PersistenceContext);
            return new LoadWorklistForEditResponse(worklist.GetRef(), adminDetail);
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Worklist)]
		public AddWorklistResponse AddWorklist(AddWorklistRequest request)
        {
            if(string.IsNullOrEmpty(request.Detail.Name))
            {
                throw new RequestValidationException(SR.ExceptionWorklistNameRequired);
            }

            WorklistAdminAssembler adminAssembler = new WorklistAdminAssembler();
            Worklist worklist = WorklistFactory.Instance.CreateWorklist(request.Detail.WorklistClass.ClassName);
            adminAssembler.UpdateWorklist(worklist, request.Detail, this.PersistenceContext);

            PersistenceContext.Lock(worklist, DirtyState.New);
            PersistenceContext.SynchState();

            return new AddWorklistResponse(adminAssembler.GetWorklistSummary(worklist, this.PersistenceContext));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Worklist)]
		public UpdateWorklistResponse UpdateWorklist(UpdateWorklistRequest request)
        {
            Worklist worklist = this.PersistenceContext.Load<Worklist>(request.EntityRef);

            WorklistAdminAssembler adminAssembler = new WorklistAdminAssembler();
            adminAssembler.UpdateWorklist(worklist, request.Detail, this.PersistenceContext);

            return new UpdateWorklistResponse(adminAssembler.GetWorklistSummary(worklist, this.PersistenceContext));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.Worklist)]
		public DeleteWorklistResponse DeleteWorklist(DeleteWorklistRequest request)
        {
			try
			{
				IWorklistBroker broker = PersistenceContext.GetBroker<IWorklistBroker>();
				Worklist item = broker.Load(request.WorklistRef, EntityLoadFlags.Proxy);
				broker.Delete(item);
				PersistenceContext.SynchState();
				return new DeleteWorklistResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, typeof(Worklist).Name));
			}
        }

        #endregion

        private List<Type> ListClassesHelper(List<string> classNames, List<string> categories, bool includeStatic)
        {
            List<Type> worklistClasses = new List<Type>(WorklistFactory.Instance.ListWorklistClasses(true));

            // optionally filter classes by class name
            if (classNames != null && classNames.Count > 0)
            {
                worklistClasses = CollectionUtils.Select(worklistClasses,
                    delegate(Type t) { return classNames.Contains(Worklist.GetClassName(t)); });
            }

            // optionally filter classes by category
            if (categories != null && categories.Count > 0)
            {
                worklistClasses = CollectionUtils.Select(worklistClasses,
                    delegate(Type t) { return categories.Contains(Worklist.GetCategory(t)); });
            }

            // optionally exclude static
            if (!includeStatic)
            {
                worklistClasses = CollectionUtils.Select(worklistClasses,
                    delegate(Type t) { return !Worklist.GetIsStatic(t); });
            }

            return worklistClasses;
        }


    }
}
