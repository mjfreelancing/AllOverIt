using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using GraphqlSchema.Schema.Types;

namespace GraphqlSchema.Schema
{
    internal interface IAppSyncDemoSubscriptionDefinition : ISubscriptionDefinition
    {
        [SubscriptionMutation(nameof(IAppSyncDemoMutationDefinition.AddCountry))]
        [SchemaTypeRequired]
        ICountry AddedCountry(GraphqlTypeId code);
    }
}