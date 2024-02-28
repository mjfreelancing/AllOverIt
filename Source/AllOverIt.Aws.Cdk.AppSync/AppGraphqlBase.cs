using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using Amazon.CDK.AWS.AppSync;
using Constructs;

namespace AllOverIt.Aws.Cdk.AppSync
{
    /// <summary>A base class for generating AppSync Graphql constructs.</summary>
    public abstract class AppGraphqlBase : GraphqlApi
    {
        private readonly SchemaBuilder _schemaBuilder;

        /// <summary>Constructor.</summary>
        /// <param name="scope">The construct scope.</param>
        /// <param name="id">The construct Id.</param>
        /// <param name="apiProps">The AppSync GraphQL API properties.</param>
        protected AppGraphqlBase(Construct scope, string id, AppGraphqlProps apiProps)
            : base(scope, id, apiProps)
        {
            var dataSourceFactory = new DataSourceFactory(this, apiProps.DataSources, apiProps.EndpointLookup);

            _schemaBuilder = new SchemaBuilder(this, apiProps, dataSourceFactory);
        }

        /// <summary>Adds a Query definition to the AppSync GraphQL API.</summary>
        /// <typeparam name="TType">The interface that provides the definition for the Query.</typeparam>
        /// <returns>Returns 'this' to support a fluent syntax.</returns>
        public AppGraphqlBase AddSchemaQuery<TType>() where TType : IQueryDefinition
        {
            _schemaBuilder.AddQuery<TType>();
            return this;
        }

        /// <summary>Adds a Mutation definition to the AppSync GraphQL API.</summary>
        /// <typeparam name="TType">The interface that provides the definition for the Mutation.</typeparam>
        /// <returns>Returns 'this' to support a fluent syntax.</returns>
        public AppGraphqlBase AddSchemaMutation<TType>() where TType : IMutationDefinition
        {
            _schemaBuilder.AddMutation<TType>();
            return this;
        }

        /// <summary>Adds a Subscription definition to the AppSync GraphQL API.</summary>
        /// <typeparam name="TType">The interface that provides the definition for the Subscription.</typeparam>
        /// <returns>Returns 'this' to support a fluent syntax.</returns>
        public AppGraphqlBase AddSchemaSubscription<TType>() where TType : ISubscriptionDefinition
        {
            _schemaBuilder.AddSubscription<TType>();
            return this;
        }
    }
}