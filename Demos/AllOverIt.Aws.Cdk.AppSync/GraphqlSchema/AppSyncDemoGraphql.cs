using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;
using Constructs;
using System.Collections.Generic;
using AllOverIt.Aws.Cdk.AppSync.Resolvers;
using SystemType = System.Type;
using AllOverIt.Aws.Cdk.AppSync.DataSources;

#if DEBUG
using Amazon.CDK.AWS.Cognito;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.S3;
#endif

namespace GraphqlSchema
{
    internal sealed class AppSyncDemoGraphql : AppGraphqlBase
    {
        public AppSyncDemoGraphql(Construct scope, AppSyncDemoAppProps appProps, IAuthorizationMode authMode, IReadOnlyDictionary<SystemType, string> typeNameOverrides,
            ResolverRegistry resolverRegistry, ResolverFactory resolverFactory)
            : base(scope, "GraphQl", GetAppGraphqlProps(scope, appProps, authMode, typeNameOverrides, resolverRegistry, resolverFactory))
        {
        }

        private static AppGraphqlProps GetAppGraphqlProps(Construct scope, AppSyncDemoAppProps appProps, IAuthorizationMode authMode,
            IReadOnlyDictionary<SystemType, string> typeNameOverrides, ResolverRegistry resolverRegistry, ResolverFactory resolverFactory)
        {
            return new AppGraphqlProps
            {
                Name = $"{appProps.AppName} V{appProps.Version}",

                AuthorizationConfig = new AuthorizationConfig
                {
                    DefaultAuthorization = authMode,

#if DEBUG   // Using RELEASE mode to deploy without these (DEBUG mode is used to check Synth output)

                    // would normally pass in the additional auth modes - these have been added to show the auth directive attributes work
                    AdditionalAuthorizationModes =
                    [
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
                    ]
#endif
                },

                TypeNameOverrides = typeNameOverrides,

                ResolverRegistry = resolverRegistry,

                ResolverFactory = resolverFactory,

                EndpointLookup = new Dictionary<string, string>
                {
                    [Constants.HttpDataSource.GetCountriesUrlLookupKey] = Fn.Join("/", [Fn.ImportValue(Constants.HttpDataSource.GetCountriesUrlImportName), "lookup"])
                },

                DataSources =
                [
                    .. CreateLambdaDataSources(),
                    .. CreateHttpDataSources(),
                    .. CreateNoneDataSources(),
                    .. CreateSubscriptionDataSources()
                ]
            };
        }

        private static IEnumerable<GraphQlDataSourceBase> CreateLambdaDataSources()
        {
            yield return new LambdaGraphQlDataSource(Constants.LambdaDataSource.GetLanguages);
            yield return new LambdaGraphQlDataSource(Constants.LambdaDataSource.AddCountry);
            yield return new LambdaGraphQlDataSource(Constants.LambdaDataSource.UpdateCountry);
        }

        private static IEnumerable<GraphQlDataSourceBase> CreateHttpDataSources()
        {
            yield return new HttpGraphQlDataSource(Constants.HttpDataSource.GetPopulationUrlExplicit, "An example Http data source");
            yield return new HttpGraphQlDataSource(EndpointSource.EnvironmentVariable, Constants.HttpDataSource.GetAllContinentsUrlEnvironmentName);
            yield return new HttpGraphQlDataSource(Constants.HttpDataSource.GetLanguageUrlExplicit);
            yield return new HttpGraphQlDataSource(EndpointSource.ImportValue, Constants.HttpDataSource.GetCountriesUrlImportName);
            yield return new HttpGraphQlDataSource(EndpointSource.Lookup, Constants.HttpDataSource.GetCountriesUrlLookupKey);
        }

        private static IEnumerable<GraphQlDataSourceBase> CreateNoneDataSources()
        {
            // Demonstrating use of shared NONE datasources
            yield return new NoneGraphQlDataSource(Constants.NoneDataSource.Query);
            yield return new NoneGraphQlDataSource(Constants.NoneDataSource.Mutation);
        }

        private static IEnumerable<GraphQlDataSourceBase> CreateSubscriptionDataSources()
        {
            // Datasources are optional for subscriptions
            yield return new NoneGraphQlDataSource(Constants.SubscriptionDataSource.AddedLanguage);
        }
    }
}