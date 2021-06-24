using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using GraphqlSchema.Schema.Types;

namespace GraphqlSchema.Schema
{
    internal interface ISolarDigestSubscriptionDefinition : ISubscriptionDefinition
    {
        [SubscriptionMutation(nameof(ISolarDigestMutationDefinition.AddSite))]
        [SchemaTypeRequired]
        ISite AddedSite(string id);
    }
}