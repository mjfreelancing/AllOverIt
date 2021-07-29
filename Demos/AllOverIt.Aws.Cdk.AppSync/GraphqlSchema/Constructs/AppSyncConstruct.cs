using AllOverIt.Aws.Cdk.AppSync;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;
using GraphqlSchema.Schema;

namespace GraphqlSchema.Constructs
{
    internal sealed class AppSyncConstruct : Construct
    {
        public AppSyncConstruct(Construct scope, AppSyncDemoAppProps appProps, AuthorizationMode authMode, MappingTemplatesBase mappingTemplates)
            : base(scope, "AppSync")
        {
            var graphql = new AppSyncDemoGraphql(this, appProps, authMode, mappingTemplates);

            graphql
                .AddSchemaQuery<IAppSyncDemoQueryDefinition>()
                .AddSchemaMutation<IAppSyncDemoMutationDefinition>()
                .AddSchemaSubscription<IAppSyncDemoSubscriptionDefinition>();
        }
    }
}