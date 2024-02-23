using AllOverIt.Aws.Cdk.AppSync.Attributes.DataSources;
using Amazon.CDK.AWS.AppSync;
using System.Collections.Generic;

namespace AllOverIt.Aws.Cdk.AppSync
{
    /// <summary>Contains options for an AppSync GraphQL API with the Schema pre-configured as a <see cref="GraphqlSchema"/>.</summary>
    public sealed class AppGraphqlProps : GraphqlApiProps
    {
        /// <summary>Provides endpoint lookup values for <see cref="HttpDataSourceAttribute"/> using <see cref="EndpointSource.Lookup"/>.</summary>
        public IReadOnlyDictionary<string, string> EndpointLookup { get; init; } = new Dictionary<string, string>();

        /// <summary>Constructor.</summary>
        public AppGraphqlProps()
        {
            Definition = Definition.FromSchema(new GraphqlSchema());
        }

        internal GraphqlSchema GetGraphqlSchema()
        {
            return Definition.Schema as GraphqlSchema;
        }
    }
}