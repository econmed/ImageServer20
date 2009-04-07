using System;
using System.Security;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;

namespace ClearCanvas.ImageServer.Common.Authentication
{
    /// <summary>
    /// Login service
    /// </summary>
    public sealed class LoginService : IDisposable
    {
        public SessionInfo Login(string userName, string password)
        {
            SessionInfo session = null;
            
            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService  service)
                    {
                        try
                        {
                            InitiateSessionRequest request = new InitiateSessionRequest(userName, password);
                            request.GetAuthorizations = true;
                            InitiateSessionResponse response = service.InitiateSession(request);

                            if (response != null)
                            {
                                LoginCredentials credentials = new LoginCredentials();
                                credentials.UserName = userName;
                                credentials.DisplayName = response.DisplayName;
                                credentials.SessionToken = response.SessionToken;
                                credentials.Authorities = response.AuthorityTokens;
                                CustomPrincipal user =
                                    new CustomPrincipal(new CustomIdentity(userName, response.DisplayName),
                                                        credentials);
                                session = new SessionInfo(user);
                            }
                            else
                            {
                                throw new SecurityException();
                            }
                        }
                        catch (FaultException<PasswordExpiredException> ex)
                        {
                            throw ex.Detail;
                        }
                    }
                );

            return session;
        }

        public void Logout(SessionInfo session)
        {
            TerminateSessionRequest request =
                new TerminateSessionRequest(session.User.Identity.Name, session.Credentials.SessionToken);

            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                    {
                        service.TerminateSession(request);
                    });
        }

        public void Validate(SessionInfo session)
        {
            ValidateSessionRequest request = new ValidateSessionRequest(session.User.Identity.Name, session.Credentials.SessionToken);
            request.GetAuthorizations = true;

            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                    {
                        ValidateSessionResponse response = service.ValidateSession(request);
                        // update session info
                        session.Credentials.Authorities = response.AuthorityTokens;
                        session.Credentials.SessionToken = response.SessionToken;
                    });

            
        }

        public void ChangePassword(string userName, string oldPassword, string newPassword)
        {

            ChangePasswordRequest request = new ChangePasswordRequest(userName, oldPassword, newPassword);
            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                    {
                        service.ChangePassword(request);
                           
                    });
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}