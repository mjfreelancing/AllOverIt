using AllOverIt.Aws.Cdk.AppSync.MappingTemplates;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;
using GraphqlSchema.Schema;

namespace GraphqlSchema
{
    internal sealed class AppSync : Construct
    {
        public AppSync(Construct scope, SolarDigestAppProps appProps, AuthorizationMode authMode, IMappingTemplates mappingTemplates)
            : base(scope, "AppSync")
        {
            var graphql = new SolarDigestGraphql(this, appProps, authMode, mappingTemplates);

            graphql
                .AddSchemaQuery<IAppSyncDemoQueryDefinition>()
                .AddSchemaMutation<IAppSyncDemoMutationDefinition>()
                .AddSchemaSubscription<IAppSyncDemoSubscriptionDefinition>();
        }
    }
}