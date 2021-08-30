using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;
using Amazon.CDK.AWS.Cognito;

namespace GraphqlSchema
{
    internal sealed class AppSyncDemoGraphql : AppGraphqlBase
    {
        public AppSyncDemoGraphql(Construct scope, AppSyncDemoAppProps appProps, IAuthorizationMode authMode, MappingTemplates mappingTemplates,
            MappingTypeFactory mappingTypeFactory)
            : base(scope, "GraphQl", GetGraphqlApiProps(scope, appProps, authMode), mappingTemplates, mappingTypeFactory)
        {
        }

        private static GraphqlApiProps GetGraphqlApiProps(Construct scope, AppSyncDemoAppProps appProps, IAuthorizationMode authMode)
        {
            return new GraphqlApiProps
            {
                Name = $"{appProps.AppName} V{appProps.Version}",

                AuthorizationConfig = new AuthorizationConfig
                {
                    DefaultAuthorization = authMode,

                    // would normally pass in the additional auth modes - these have been added to show the auth directive attributes work
                    AdditionalAuthorizationModes = new IAuthorizationMode[]
                        {
                            new AuthorizationMode
                            {
                                AuthorizationType = AuthorizationType.USER_POOL,
                                UserPoolConfig = new UserPoolConfig
                                {
                                    UserPool = new UserPool(scope, "SomeUserPool")
                                }
                            },
                            new AuthorizationMode
                            {
                                AuthorizationType = AuthorizationType.OIDC,
                                OpenIdConnectConfig = new OpenIdConnectConfig
                                {
                                    OidcProvider = "https://domain.com"
                                }
                            },
                            new AuthorizationMode
                            {
                                AuthorizationType = AuthorizationType.IAM
                            }
                        }
                }
            };
        }
    }
}