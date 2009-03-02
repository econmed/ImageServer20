using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Services.Admin;
using ClearCanvas.ImageServer.Common.Services.Login;
using ClearCanvas.ImageServer.Common.Authentication;
using AuthorityTokens=ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens;

namespace ClearCanvas.ImageServer.Utilities
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class AuthorityGroupImporter : IApplicationRoot
    {
        SessionInfo _session;
        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            CommandLine cmd = new CommandLine(args);
            string user = cmd.Named["u"];
            string pwd = cmd.Named["p"];

            Platform.GetService<ILoginService>(delegate(ILoginService service)
                                                   {
                                                       try
                                                       {
                                                           Console.WriteLine("Login as {0}...", user);
                                                           _session = service.Login(user, pwd);
                                                       }
                                                       catch (PasswordExpiredException ex)
                                                       {
                                                           Console.WriteLine("Password has expired. Reseting password..");
                                                           service.ChangePassword(user, pwd, "clearcanvas123");
                                                           service.ChangePassword(user, "clearcanvas123", pwd);
                                                           Console.WriteLine("Attempt to login again...");
                                                           _session = service.Login(user, pwd);
                                                       }
                                                   });

            AuthorityGroupDefinition[] groupDefs = AuthorityGroupSetup.GetDefaultAuthorityGroups();
            Import(groupDefs);
        }

        #endregion

        private void Import(IEnumerable<AuthorityGroupDefinition> groupDefs)
        {
            List<AuthorityGroupDetail> groups = CollectionUtils.Map<AuthorityGroupDefinition, AuthorityGroupDetail>(
                groupDefs,
                delegate(AuthorityGroupDefinition groupDef)
                {
                    AuthorityGroupDetail group = new AuthorityGroupDetail();
                    group.Name = groupDef.Name;
                    group.AuthorityTokens = CollectionUtils.Map<string, AuthorityTokenSummary>(
                        groupDef.Tokens,
                        delegate(string tokenName)
                            {
                                AuthorityTokenSummary token = new AuthorityTokenSummary(tokenName, String.Empty);
                                return token;
                            });
                    return group;
                }
                );

            Console.WriteLine("*********************************************");
            Console.WriteLine("The following groups will be created:");
            foreach(AuthorityGroupDetail group in groups)
            {
                Console.WriteLine(group.Name);
            }
            Console.WriteLine("*********************************************");

            Platform.GetService<IAuthorityAdminService>(
                delegate(IAuthorityAdminService service)
                {
                    service.Credentials = _session.Credentials;
                    Console.WriteLine("Loading default groups into the server...");
                    if (!service.ImportAuthorityGroups(groups))
                        Console.WriteLine("Unable to load default user groups. See server log for more info.");
                    else
                        Console.WriteLine("Groups are successfully loaded.");

                });
        }

    }
}
