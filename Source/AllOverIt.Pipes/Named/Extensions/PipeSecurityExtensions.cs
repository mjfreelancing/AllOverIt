using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace AllOverIt.Pipes.Named.Extensions
{
    /// <summary>Provides extension methods for <see cref="PipeSecurity"/>.</summary>
    [ExcludeFromCodeCoverage]
    public static class PipeSecurityExtensions
    {
        /// <summary>Adds an access rule to a Discretionary Access Control List (DACL) that is associated with the provided <see cref="PipeSecurity"/>.</summary>
        /// <param name="pipeSecurity">The <see cref="PipeSecurity"/> instance.</param>
        /// <param name="wellKnownSid">The well known security identifier.</param>
        /// <param name="accessRight">The access right to apply.</param>
        /// <param name="accessControl">Indicates if the access rule is to be allowed or denied.</param>
        /// <returns>The modified <see cref="PipeSecurity"/> instance.</returns>
        public static PipeSecurity AddIdentityAccessRule(this PipeSecurity pipeSecurity, WellKnownSidType wellKnownSid, PipeAccessRights accessRight,
            AccessControlType accessControl)
        {
            // Get the identity of the user account to grant or deny access
            var identity = new SecurityIdentifier(wellKnownSid, null);

            return pipeSecurity.AddIdentityAccessRule(identity, accessRight, accessControl);
        }

        /// <summary>Adds an access rule to a Discretionary Access Control List (DACL) that is associated with the provided <see cref="PipeSecurity"/>.</summary>
        /// <param name="pipeSecurity">The <see cref="PipeSecurity"/> instance.</param>
        /// <param name="wellKnownSid">The well known security identifier.</param>
        /// <param name="domainSid">A domain security identifier. This can be <see langword="null"/>.</param>
        /// <param name="accessRight">The access right to apply.</param>
        /// <param name="accessControl">Indicates if the access rule is to be allowed or denied.</param>
        /// <returns>The modified <see cref="PipeSecurity"/> instance.</returns>
        public static PipeSecurity AddIdentityAccessRule(this PipeSecurity pipeSecurity, WellKnownSidType wellKnownSid, SecurityIdentifier domainSid,
            PipeAccessRights accessRight, AccessControlType accessControl)
        {
            // Get the identity of the user account to grant or deny access
            var identity = new SecurityIdentifier(wellKnownSid, domainSid);

            return pipeSecurity.AddIdentityAccessRule(identity, accessRight, accessControl);
        }

        private static PipeSecurity AddIdentityAccessRule(this PipeSecurity pipeSecurity, SecurityIdentifier identity, PipeAccessRights accessRight,
            AccessControlType accessControl)
        {
            // Create an access rule to grant or deny specific rights to the user
            var accessRule = new PipeAccessRule(identity, accessRight, accessControl);

            pipeSecurity.AddAccessRule(accessRule);

            return pipeSecurity;
        }
    }
}