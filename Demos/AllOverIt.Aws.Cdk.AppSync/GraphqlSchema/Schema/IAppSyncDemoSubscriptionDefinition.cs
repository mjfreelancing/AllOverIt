using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using GraphqlSchema.Schema.Resolvers.Subscripion;
using GraphqlSchema.Schema.Types;

namespace GraphqlSchema.Schema
{
    internal interface IAppSyncDemoSubscriptionDefinition : ISubscriptionDefinition
    {
        [SubscriptionMutation(nameof(IAppSyncDemoMutationDefinition.AddCountry))]
        // Not adding a resolver to this subscription
        ICountry AddedCountry(GraphqlTypeId code);          // Optional, so all countries added will be reported.

        [SubscriptionMutation(nameof(IAppSyncDemoMutationDefinition.AddLanguage))]
        [UnitResolver(typeof(AddedLanguageResolver), Constants.SubscriptionDataSource.AddedLanguage)]
        ILanguage AddedLanguage(GraphqlTypeId code);        // Only providing the ability to (optionally) filter by code.
    }


}