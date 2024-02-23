using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using Amazon.CDK.AWS.AppSync;
using Constructs;
using System.Collections.Generic;
using SystemType = System.Type;

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
        /// <param name="typeNameOverrides">Provides name overrides for types discovered without a <see cref="SchemaTypeBaseAttribute"/>,
        /// such as <see cref="SchemaEnumAttribute"/>. This would normally be used with other types, such as enumerations, defined in a shared
        /// assembly.</param>
        /// <param name="mappingTemplates">Contains request and response mapping templates for datasources that are not provided
        /// a mapping type. If null then an an internal version will be created.</param>
        /// <param name="mappingTypeFactory">Contains registrations for mapping types (defined on a DataSource) that do not have
        /// a default constructor because arguments need to be provided at runtime.</param>
        protected AppGraphqlBase(Construct scope, string id, AppGraphqlProps apiProps, IReadOnlyDictionary<SystemType, string> typeNameOverrides = default,
            MappingTemplates mappingTemplates = default, MappingTypeFactory mappingTypeFactory = default)
            : base(scope, id, apiProps)
        {
            typeNameOverrides ??= new Dictionary<SystemType, string>();
            mappingTemplates ??= new MappingTemplates();
            mappingTypeFactory ??= new MappingTypeFactory();

            var dataSourceFactory = new DataSourceFactory(this, apiProps.EndpointLookup);

            var schema = apiProps.GetGraphqlSchema();

            var gqlTypeCache = new GraphqlTypeStore(this, schema, typeNameOverrides, mappingTemplates, mappingTypeFactory, dataSourceFactory);

            _schemaBuilder = new SchemaBuilder(this, schema, mappingTemplates, mappingTypeFactory, gqlTypeCache, dataSourceFactory);
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