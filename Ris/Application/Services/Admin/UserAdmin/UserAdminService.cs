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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using System.ServiceModel;
using System.Security.Permissions;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Enterprise.Authentication.Imex;

namespace ClearCanvas.Ris.Application.Services.Admin.UserAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IUserAdminService))]
    public class UserAdminService : ApplicationServiceBase, IUserAdminService
    {
        #region IUserAdminService Members

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
        public ListUsersResponse ListUsers(ListUsersRequest request)
        {
            UserSearchCriteria criteria = new UserSearchCriteria();

            UserAssembler assembler = new UserAssembler();
            List<UserSummary> userSummaries = CollectionUtils.Map<User, UserSummary, List<UserSummary>>(
                PersistenceContext.GetBroker<IUserBroker>().Find(criteria, request.Page),
                delegate(User user)
                {
                    return assembler.GetUserSummary(user);
                });
            return new ListUsersResponse(userSummaries);
        }

        [ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public LoadUserForEditResponse LoadUserForEdit(LoadUserForEditRequest request)
        {
            User user = FindUserByName(request.UserName);
            Staff staff = null;
            try
            {
                StaffSearchCriteria where = new StaffSearchCriteria();
                where.UserName.EqualTo(user.UserName);

                IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
                staff = staffBroker.FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                // no staff for user
            }

            UserAssembler assembler = new UserAssembler();
            return new LoadUserForEditResponse(assembler.GetUserDetail(user, staff));
        }


        [ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request)
        {
            AuthorityGroupSearchCriteria criteria = new AuthorityGroupSearchCriteria();

            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            List<AuthorityGroupSummary> authorityGroups = CollectionUtils.Map<AuthorityGroup, AuthorityGroupSummary, List<AuthorityGroupSummary>>(
                PersistenceContext.GetBroker<IAuthorityGroupBroker>().Find(criteria, request.Page),
                delegate(AuthorityGroup authorityGroup)
                {
                    return assembler.GetAuthorityGroupSummary(authorityGroup);
                });
            return new ListAuthorityGroupsResponse(authorityGroups);
        }

        [ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public LoadAuthorityGroupForEditResponse LoadAuthorityGroupForEdit(LoadAuthorityGroupForEditRequest request)
        {
            AuthorityGroup authorityGroup = FindAuthorityGroupByName(request.AuthorityGroupName);
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            return new LoadAuthorityGroupForEditResponse(authorityGroup.GetRef(), assembler.GetAuthorityGroupDetail(authorityGroup));
        }

        [ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public ListAuthorityTokensResponse ListAuthorityTokens(ListAuthorityTokensRequest request)
        {
            AuthorityTokenAssembler assembler = new AuthorityTokenAssembler();
            List<AuthorityTokenSummary> authorityTokens = CollectionUtils.Map<AuthorityToken, AuthorityTokenSummary, List<AuthorityTokenSummary>>(
                PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindAll(),
                delegate(AuthorityToken authorityToken)
                {
                    return assembler.GetAuthorityTokenSummary(authorityToken);
                });
            return new ListAuthorityTokensResponse(authorityTokens);
        }

    	[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public AddUserResponse AddUser(AddUserRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.UserDetail, "UserDetail");

            UserDetail userDetail = request.UserDetail;
            User user = User.CreateNewUser(
                new UserInfo(userDetail.UserName, userDetail.DisplayName, userDetail.ValidFrom, userDetail.ValidUntil));

            // copy other info such as authority groups
            UserAssembler assembler = new UserAssembler();
            assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

            PersistenceContext.Lock(user, DirtyState.New);

            // create staff association
            if (request.UserDetail.StaffRef != null)
            {
                IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
                Staff staff = staffBroker.Load(request.UserDetail.StaffRef);
                staff.UserName = user.UserName;
            }

            PersistenceContext.SynchState();

            return new AddUserResponse(user.GetRef(), assembler.GetUserSummary(user));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public UpdateUserResponse UpdateUser(UpdateUserRequest request)
        {
            User user = FindUserByName(request.UserDetail.UserName);

            // update user account info
            UserAssembler assembler = new UserAssembler();
            assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

            // reset password if requested
            if (request.UserDetail.ResetPassword)
                user.ResetPassword();

            IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
            try
            {
                // dissociate any previously associated staff
                StaffSearchCriteria where = new StaffSearchCriteria();
                where.UserName.EqualTo(user.UserName);
                Staff staff = staffBroker.FindOne(where);
                staff.UserName = null;
            }
            catch (EntityNotFoundException)
            {
                // no previously associated staff
            }

            // create staff association
            if (request.UserDetail.StaffRef != null)
            {
                Staff staff = staffBroker.Load(request.UserDetail.StaffRef);
                staff.UserName = user.UserName;
            }

            return new UpdateUserResponse(assembler.GetUserSummary(user));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public ResetUserPasswordResponse ResetUserPassword(ResetUserPasswordRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.UserName, "UserName");

            User user = FindUserByName(request.UserName);
            user.ResetPassword();

            UserAssembler assembler = new UserAssembler();
            return new ResetUserPasswordResponse(assembler.GetUserSummary(user));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public AddAuthorityGroupResponse AddAuthorityGroup(AddAuthorityGroupRequest request)
        {
            AuthorityGroup authorityGroup = new AuthorityGroup();
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            assembler.UpdateAuthorityGroup(authorityGroup, request.AuthorityGroupDetail, PersistenceContext);

            PersistenceContext.Lock(authorityGroup, DirtyState.New);
            PersistenceContext.SynchState();

            return new AddAuthorityGroupResponse(assembler.GetAuthorityGroupSummary(authorityGroup));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public UpdateAuthorityGroupResponse UpdateAuthorityGroup(UpdateAuthorityGroupRequest request)
        {
            AuthorityGroup authorityGroup = FindAuthorityGroupByName(request.AuthorityGroupDetail.Name);
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            assembler.UpdateAuthorityGroup(authorityGroup, request.AuthorityGroupDetail, PersistenceContext);

            PersistenceContext.Lock(authorityGroup, DirtyState.Dirty);

            return new UpdateAuthorityGroupResponse(assembler.GetAuthorityGroupSummary(authorityGroup));
        }

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public ImportAuthorityTokensResponse ImportAuthorityTokens(ImportAuthorityTokensRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Tokens, "Tokens");

			if(request.Tokens.Count > 0)
			{
				AuthorityTokenImporter importer = new AuthorityTokenImporter();
				importer.Import(
					CollectionUtils.Map<AuthorityTokenSummary, AuthorityTokenDefinition>(request.Tokens,
						delegate(AuthorityTokenSummary s) { return new AuthorityTokenDefinition(s.Name, s.Description); }),
						(IUpdateContext)PersistenceContext);

			}

			return new ImportAuthorityTokensResponse();
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.AuthorityGroup)]
		public ImportAuthorityGroupsResponse ImportAuthorityGroups(ImportAuthorityGroupsRequest request)
    	{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.AuthorityGroups, "AuthorityGroups");

			if (request.AuthorityGroups.Count > 0)
			{
				AuthorityGroupImporter importer = new AuthorityGroupImporter();
				importer.Import(
					CollectionUtils.Map<AuthorityGroupDetail, AuthorityGroupDefinition>(request.AuthorityGroups,
						delegate(AuthorityGroupDetail g)
						{
							return new AuthorityGroupDefinition(g.Name,
								CollectionUtils.Map<AuthorityTokenSummary, string>(g.AuthorityTokens,
								   delegate(AuthorityTokenSummary s) { return s.Name; }).ToArray());
						}),
						(IUpdateContext)PersistenceContext);

			}

			return new ImportAuthorityGroupsResponse();
		}

    	#endregion

        private AuthorityGroup FindAuthorityGroupByName(string name)
        {
            try
            {
                AuthorityGroupSearchCriteria where = new AuthorityGroupSearchCriteria();
                where.Name.EqualTo(name);

                return PersistenceContext.GetBroker<IAuthorityGroupBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                throw new RequestValidationException(string.Format("{0} is not a valid authority group.", name));
            }
        }

        private User FindUserByName(string name)
        {
            try
            {
                UserSearchCriteria where = new UserSearchCriteria();
                where.UserName.EqualTo(name);

                return PersistenceContext.GetBroker<IUserBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                throw new RequestValidationException(string.Format("{0} is not a valid user name.", name));
            }
        }

    }
}
