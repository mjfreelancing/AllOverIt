﻿using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using GraphqlSchema.Schema.Types;

namespace GraphqlSchema.Schema
{
    internal interface IAppSyncDemoSubscriptionDefinition : ISubscriptionDefinition
    {
        [SubscriptionMutation(nameof(IAppSyncDemoMutationDefinition.AddCountry))]   // mutation(s) triggering the subscription
        [NoneDataSource(Constants.AppName, nameof(AddedCountry))]                   // the handling datasource
        [SchemaTypeRequired]
        ICountry AddedCountry(GraphqlTypeId code);                                  // optional, so all countries added will be reported
    }
}