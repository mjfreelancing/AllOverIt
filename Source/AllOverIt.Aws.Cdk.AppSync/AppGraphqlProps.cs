using AllOverIt.Aws.Cdk.AppSync.DataSources;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Resolvers;
using Amazon.CDK.AWS.AppSync;

using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync
{
    /// <summary>Contains options for an AppSync GraphQL API with the Schema pre-configured as a <see cref="GraphqlSchema"/>.</summary>
    public sealed class AppGraphqlProps : GraphqlApiProps, IAppGraphqlProps
    {
        /// <inheritdoc />
        public GraphqlDataSourceBase[] DataSources { get; init; } = [];

        /// <inheritdoc />
        public IReadOnlyDictionary<SystemType, string> TypeNameOverrides { get; init; } = new Dictionary<SystemType, string>();

        /// <inheritdoc />
        public ResolverRegistry ResolverRegistry { get; init; } = new ResolverRegistry();

        /// <inheritdoc />
        public ResolverFactory ResolverFactory { get; init; } = new ResolverFactory();

        /// <summary>Constructor.</summary>
        public AppGraphqlProps()
        {
            Definition = Definition.FromSchema(new GraphqlSchema());
        }

        internal GraphqlSchema GetGraphqlSchema()
        {
            return (Definition!.Schema as GraphqlSchema)!;
        }
    }
}