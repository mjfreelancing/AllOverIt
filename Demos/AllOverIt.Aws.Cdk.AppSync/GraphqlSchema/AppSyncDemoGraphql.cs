using AllOverIt.Aws.Cdk.AppSync;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;

namespace GraphqlSchema
{
    internal sealed class AppSyncDemoGraphql : AppGraphqlBase
    {
        public AppSyncDemoGraphql(Construct scope, AppSyncDemoAppProps appProps, IAuthorizationMode authMode, MappingTemplatesBase mappingTemplates)
            : base(scope, "GraphQl", GetGraphqlApiProps(appProps, authMode), mappingTemplates)
        {
        }

        private static GraphqlApiProps GetGraphqlApiProps(AppSyncDemoAppProps appProps, IAuthorizationMode authMode)
        {
            return new GraphqlApiProps
            {
                Name = $"{appProps.AppName} V{appProps.Version}",
                AuthorizationConfig = new AuthorizationConfig { DefaultAuthorization = authMode }
            };
        }
    }
}