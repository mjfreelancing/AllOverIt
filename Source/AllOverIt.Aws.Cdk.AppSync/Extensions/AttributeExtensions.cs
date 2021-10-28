using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using Amazon.CDK.AWS.AppSync;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class AttributeExtensions
    {
        public static Directive[] GetAuthDirectivesOrDefault(this IEnumerable<AuthDirectiveBaseAttribute> attributes)
        {
            var directives = new List<Directive>();

            foreach (var attribute in attributes)
            {
                var directive = attribute.Mode switch
                {
                    AuthDirectiveMode.Oidc => Directive.Oidc(),
                    AuthDirectiveMode.ApiKey => Directive.ApiKey(),
                    AuthDirectiveMode.Cognito => Directive.Cognito((attribute as AuthCognitoDirectiveAttribute)!.Groups.ToArray()),
                    AuthDirectiveMode.Iam => Directive.Iam(),
                    _ => throw new ArgumentOutOfRangeException($"Unknown auth mode '{attribute.Mode}'")
                };

                directives.Add(directive);
            }

            return directives.Any()
                ? directives.ToArray()
                : null;
        }
    }
}