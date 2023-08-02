using Amazon.CDK.AWS.AppSync;
using Cdklabs.AwsCdkAppsyncUtils;
using System.Collections.Generic;

namespace AllOverIt.Aws.Cdk.AppSync
{
    /// <summary>Contains options for an AppSync GraphQL API with the Schema pre-configured as a <see cref="CodeFirstSchema"/>.</summary>
    public sealed class AppGraphqlProps : GraphqlApiProps
    {
        public IReadOnlyDictionary<string, string> EndpointLookup { get; init; } = new Dictionary<string, string>();

        /// <summary>Constructor.</summary>
        public AppGraphqlProps()
        {
            Schema = new CodeFirstSchema();
        }

        internal CodeFirstSchema GetCodeFirstSchema()
        {
            return Schema as CodeFirstSchema;
        }
    }
}