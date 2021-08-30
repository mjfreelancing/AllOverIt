using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using Amazon.CDK.AWS.AppSync;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    public static class AttributeExtensions
    {
        public static IReadOnlyCollection<Directive> GetAuthDirectives(this IReadOnlyCollection<AuthDirectiveBaseAttribute> attributes)
        {
            var directives = new List<Directive>();

            foreach (var attribute in attributes)
            {
                var directive = attribute.Mode switch
                {
                    AuthDirectiveMode.Oidc => Directive.Oidc(),
                    AuthDirectiveMode.ApiKey => Directive.ApiKey(),
                    AuthDirectiveMode.Cognito => Directive.Cognito((attribute as AuthCognitoDirectiveAttribute)!.Groups.ToArray()),
                    AuthDirectiveMode.Iam => Directive.Iam()
                };

                directives.Add(directive);
            }

            return directives;
        }
    }
}