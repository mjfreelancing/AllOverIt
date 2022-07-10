using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Directives
{
    /// <summary>Applies a Cognito Authorization directive.</summary>
    public sealed class AuthCognitoDirectiveAttribute : AuthDirectiveBaseAttribute
    {
        /// <summary>One or more Cognito groups.</summary>
        public IReadOnlyCollection<string> Groups { get; }

        /// <summary>Constructor.</summary>
        /// <param name="groups">One or more Cognito groups.</param>
        public AuthCognitoDirectiveAttribute(params string[] groups)
            : base(AuthDirectiveMode.Cognito)
        {
            Groups = groups?.AsReadOnlyCollection();

            if (Groups.IsNullOrEmpty())
            {
                throw new SchemaException("At least one Cognito user group must be povided.");
            }
        }
    }
}