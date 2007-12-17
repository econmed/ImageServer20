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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Enterprise.Authentication.Setup
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class SetupApplication : IApplicationRoot
    {
        private const string SysAdminUserName = "sa";
        private const string SysAdminGroup = "Administrators";

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
            {
                // import authority tokens
                AuthorityTokenImporter tokenImporter = new AuthorityTokenImporter();
                IList<AuthorityToken> allTokens = tokenImporter.ImportFromPlugins((IUpdateContext)PersistenceScope.Current, Console.Out);


                // create the sys admin group, which has all tokens assigned by default
                string[] tokenStrings = CollectionUtils.Map<AuthorityToken, string, List<string>>(allTokens,
                   delegate(AuthorityToken t) { return t.Name; }).ToArray();
                AuthorityGroupDefinition adminGroupDef = new AuthorityGroupDefinition(SysAdminGroup, tokenStrings);
                AuthorityGroupImporter groupImporter = new AuthorityGroupImporter();
                groupImporter.Import(new AuthorityGroupDefinition[] { adminGroupDef }, (IUpdateContext)PersistenceScope.Current, Console.Out);

                // import any other authority groups defined in other plugins
                IList<AuthorityGroup> allGroups = groupImporter.ImportFromPlugins((IUpdateContext)PersistenceScope.Current, Console.Out);

                // find the admin group that was just created
                AuthorityGroup adminGroup = CollectionUtils.SelectFirst<AuthorityGroup>(allGroups,
                    delegate(AuthorityGroup g) { return g.Name == SysAdminGroup; });

                // create the "sa" user
                CreateSysAdminUser(adminGroup, PersistenceScope.Current, Console.Out);

                scope.Complete();
            }
        }

        public void CreateSysAdminUser(AuthorityGroup adminGroup, IPersistenceContext context, TextWriter log)
        {
            try
            {
                // create the sa user, if doesn't already exist
                IUserBroker userBroker = context.GetBroker<IUserBroker>();
                UserSearchCriteria where = new UserSearchCriteria();
                where.UserName.EqualTo(SysAdminUserName);
                userBroker.FindOne(where);

                log.WriteLine(string.Format("User '{0}' already exists.", SysAdminUserName));
            }
            catch (EntityNotFoundException)
            {
                User saUser = new User();
                saUser.UserName = SysAdminUserName;
                saUser.AuthorityGroups.Add(adminGroup);

                context.Lock(saUser, DirtyState.New);
            }
        }

        #endregion
    }
}
