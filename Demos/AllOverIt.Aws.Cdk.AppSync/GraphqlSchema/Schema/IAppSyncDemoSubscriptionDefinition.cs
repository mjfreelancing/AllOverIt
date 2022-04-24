using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Attributes.DataSources;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using GraphqlSchema.Schema.Types;
using GraphqlSchema.Schema.Types.Globe;

namespace GraphqlSchema.Schema
{
    internal interface IAppSyncDemoSubscriptionDefinition : ISubscriptionDefinition
    {
        [SubscriptionMutation(nameof(IAppSyncDemoMutationDefinition.AddCountry))]
        [SubscriptionDataSource(nameof(AddedCountry))]
        ICountry AddedCountry(GraphqlTypeId code);          // optional, so all countries added will be reported

        [SubscriptionMutation(nameof(IAppSyncDemoMutationDefinition.AddLanguage))]   
        [SubscriptionDataSource(nameof(AddedLanguage))]
        ILanguage AddedLanguage(GraphqlTypeId code);                                // only providing the ability to (optionally) filter by code
    }
}