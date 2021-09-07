using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Datasources;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using GraphqlSchema.Schema.Types;

namespace GraphqlSchema.Schema
{
    internal interface IAppSyncDemoSubscriptionDefinition : ISubscriptionDefinition
    {
        [SubscriptionMutation(nameof(IAppSyncDemoMutationDefinition.AddCountry))]
        [SubscriptionDataSource(nameof(AddedCountry))]
        ICountry AddedCountry(GraphqlTypeId code);          // optional, so all countries added will be reported

        [SubscriptionMutation(nameof(IAppSyncDemoMutationDefinition.AddLanguage))]   
        [SubscriptionDataSource(nameof(AddedLanguage))]
        ILanguage AddedLanguage(GraphqlTypeId code);                                 
    }
}