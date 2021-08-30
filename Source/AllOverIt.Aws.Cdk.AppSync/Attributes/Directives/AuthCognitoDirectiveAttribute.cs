using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Directives
{
    public sealed class AuthCognitoDirectiveAttribute : AuthDirectiveBaseAttribute
    {
        public IReadOnlyCollection<string> Groups { get; }

        public AuthCognitoDirectiveAttribute(params string[] groups)
            : base(AuthDirectiveMode.Cognito)
        {
            Groups = groups.AsReadOnlyCollection();

            if (Groups.IsNullOrEmpty())
            {
                throw new SchemaException("At least one Cognito user group must be povided.");
            }
        }
    }
}