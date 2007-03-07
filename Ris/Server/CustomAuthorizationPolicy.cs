using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security.Principal;

namespace ClearCanvas.Ris.Server
{
    class CustomAuthorizationPolicy : IAuthorizationPolicy
    {
        string id = Guid.NewGuid().ToString();

        public string Id
        {
            get { return this.id; }
        }

        public ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }

        public bool Evaluate(EvaluationContext context, ref object state)
        {
            object obj;
            if (!context.Properties.TryGetValue("Identities", out obj))
                return false;

            IList<IIdentity> identities = obj as IList<IIdentity>;
            if (obj == null || identities.Count <= 0)
                return false;

            context.Properties["Principal"] = new GenericPrincipal(identities[0], new string[] { "everyone" });
            return true;
        }
    }
}
