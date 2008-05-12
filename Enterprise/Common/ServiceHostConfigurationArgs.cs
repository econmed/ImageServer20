using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Selectors;
using System.IdentityModel.Policy;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Arguments for configuration of a service host.
	/// </summary>
    public struct ServiceHostConfigurationArgs
    {
        public ServiceHostConfigurationArgs(Type serviceContract, Uri hostUri, bool authenticated,
			int maxReceivedMessageSize, UserNamePasswordValidator userNamePasswordValidator, IAuthorizationPolicy authorizationPolicy)
        {
            ServiceContract = serviceContract;
            HostUri = hostUri;
            Authenticated = authenticated;
            MaxReceivedMessageSize = maxReceivedMessageSize;
			UserNamePasswordValidator = userNamePasswordValidator;
        	AuthorizationPolicy = authorizationPolicy;
        }

		/// <summary>
		/// The service contract for which the host is created.
		/// </summary>
        public Type ServiceContract;

		/// <summary>
		/// The URI on which the service is being exposed.
		/// </summary>
        public Uri HostUri;

		/// <summary>
		/// A value indicating whether the service is authenticated, or allows anonymous access.
		/// </summary>
        public bool Authenticated;

		/// <summary>
		/// The maximum allowable size of received messages, in bytes.
		/// </summary>
        public int MaxReceivedMessageSize;

		/// <summary>
		/// An instance of a <see cref="UserNamePasswordValidator"/>, that an authenticated service will use
		/// to perform authentication.
		/// </summary>
    	public UserNamePasswordValidator UserNamePasswordValidator;

		/// <summary>
		/// An instance of a <see cref="IAuthorizationPolicy"/>, that an authenticated service will use
		/// to perform authorization checks.
		/// </summary>
    	public IAuthorizationPolicy AuthorizationPolicy;
    }
}
