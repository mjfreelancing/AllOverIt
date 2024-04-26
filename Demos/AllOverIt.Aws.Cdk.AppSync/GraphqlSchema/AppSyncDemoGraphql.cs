using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.DataSources;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Resolvers;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;
using Constructs;

using SystemType = System.Type;

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

                DataSources =
                [
                    .. CreateLambdaDataSources(),
                    .. CreateHttpDataSources(),
                    .. CreateNoneDataSources(),
                    .. CreateSubscriptionDataSources()
                ]
            };
        }

        private static IEnumerable<GraphqlDataSourceBase> CreateLambdaDataSources()
        {
            yield return new LambdaGraphqlDataSource(Constants.LambdaDataSource.GetLanguages, Constants.LambdaDataSource.GetLanguages);
            yield return new LambdaGraphqlDataSource(Constants.LambdaDataSource.AddCountry, Constants.LambdaDataSource.AddCountry);
            yield return new LambdaGraphqlDataSource(Constants.LambdaDataSource.UpdateCountry, Constants.LambdaDataSource.UpdateCountry);
        }

        private static IEnumerable<GraphqlDataSourceBase> CreateHttpDataSources()
        {
            yield return new HttpGraphqlDataSource(Constants.HttpDataSource.GetPopulationUrl, "https://www.microsoft.com", "An example Http data source");
            yield return new HttpGraphqlDataSource(Constants.HttpDataSource.GetLanguageUrlExportName, "https://www.google.com");
            yield return new HttpGraphqlDataSource(Constants.HttpDataSource.GetCountriesUrlImportName, Fn.Join("/", [Fn.ImportValue(Constants.HttpDataSource.GetCountriesUrlImportName), "lookup"]));
            yield return new HttpGraphqlDataSource(Constants.HttpDataSource.GetCountryCodesUrl, "https://www.yahoo.com");
            yield return new HttpGraphqlDataSource(Constants.HttpDataSource.GetAllContinentsUrlEnvironmentName, System.Environment.GetEnvironmentVariable(Constants.HttpDataSource.GetAllContinentsUrlEnvironmentName));
        }

        private static IEnumerable<GraphqlDataSourceBase> CreateNoneDataSources()
        {
            // Demonstrating use of shared NONE datasources
            yield return new NoneGraphqlDataSource(Constants.NoneDataSource.Query);
            yield return new NoneGraphqlDataSource(Constants.NoneDataSource.Mutation);
        }

        private static IEnumerable<GraphqlDataSourceBase> CreateSubscriptionDataSources()
        {
            // Datasources are optional for subscriptions
            yield return new NoneGraphqlDataSource(Constants.SubscriptionDataSource.AddedLanguage);
        }
    }
}