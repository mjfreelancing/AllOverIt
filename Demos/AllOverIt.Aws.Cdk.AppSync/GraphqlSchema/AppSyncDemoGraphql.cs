using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;
using Amazon.CDK.AWS.Cognito;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.S3;
using Constructs;
using System.Collections.Generic;
using SystemType = System.Type;

namespace GraphqlSchema
{
    internal sealed class AppSyncDemoGraphql : AppGraphqlBase
    {
        public AppSyncDemoGraphql(Construct scope, AppSyncDemoAppProps appProps, IAuthorizationMode authMode, IReadOnlyDictionary<SystemType, string> typeNameOverrides,
            MappingTemplates mappingTemplates, MappingTypeFactory mappingTypeFactory)
            : base(scope, "GraphQl", GetAppGraphqlProps(scope, appProps, authMode), typeNameOverrides, mappingTemplates, mappingTypeFactory)
        {
        }

        private static AppGraphqlProps GetAppGraphqlProps(Construct scope, AppSyncDemoAppProps appProps, IAuthorizationMode authMode)
        {
            return new AppGraphqlProps
            {
                Name = $"{appProps.AppName} V{appProps.Version}",

                AuthorizationConfig = new AuthorizationConfig
                {
                    DefaultAuthorization = authMode,

#if DEBUG   // Using RELEASE mode to deploy without these (DEBUG mode is used to check Synth output)

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
                        },

                        new AuthorizationMode
                        {
                            AuthorizationType = AuthorizationType.LAMBDA,
                            LambdaAuthorizerConfig = new LambdaAuthorizerConfig
                            {
                                Handler = new Function(scope, "some_function", new FunctionProps
                                {
                                    FunctionName = $"function_name",
                                    Code = new S3Code(Bucket.FromBucketName(scope, "deploy-bucket", "bucket-name"), $"file.zip"),
                                    Handler = $"Namespace.FunctionName::HandleAsync",
                                    Runtime = Runtime.DOTNET_6,
                                    MemorySize = 512,
                                    Timeout = Duration.Seconds(900),
                                    //Environment = variables,
                                    //Vpc = Vpc,
                                    //VpcSubnets = AppSubnetSelection
                                })
                            }
                        }
                    }
#endif
                },
                EndpointLookup = new Dictionary<string, string>
                {
                    [Constants.Lookup.GetCountriesUrlKey] = Fn.Join("/", new[] { Fn.ImportValue(Constants.Import.GetCountriesUrlImportName), "lookup" })
                }
            };
        }
    }
}