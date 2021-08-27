using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;

namespace AllOverIt.Aws.Cdk.AppSync
{
    public abstract class AppGraphqlBase : GraphqlApi
    {
        private readonly SchemaBuilder _schemaBuilder;

        /// <summary>Constructor.</summary>
        /// <param name="mappingTemplates">Contains request and response mapping templates for datasources that are not provided
        /// a mapping type. If null then an an internal version will be created.</param>
        /// <param name="mappingTypeFactory">Contains registrations for mapping types (defined on a DataSource) that do not have
        /// a default constructor because arguments need to be provided at runtime.</param>
        protected AppGraphqlBase(Construct scope, string id, IGraphqlApiProps apiProps, MappingTemplates mappingTemplates = default,
            MappingTypeFactory mappingTypeFactory = default)
            : base(scope, id, apiProps)
        {
            mappingTemplates ??= new MappingTemplates();
            mappingTypeFactory ??= new MappingTypeFactory();

            var dataSourceFactory = new DataSourceFactory(this);
            var gqlTypeCache = new GraphqlTypeStore(this, mappingTemplates, mappingTypeFactory, dataSourceFactory);
            _schemaBuilder = new SchemaBuilder(this, mappingTemplates, mappingTypeFactory, gqlTypeCache, dataSourceFactory);
        }

        public AppGraphqlBase AddSchemaQuery<TType>() where TType : IQueryDefinition
        {
            _schemaBuilder.AddQuery<TType>();
            return this;
        }

        public AppGraphqlBase AddSchemaMutation<TType>() where TType : IMutationDefinition
        {
            _schemaBuilder.AddMutation<TType>();
            return this;
        }

        public AppGraphqlBase AddSchemaSubscription<TType>() where TType : ISubscriptionDefinition
        {
            _schemaBuilder.AddSubscription<TType>();
            return this;
        }
    }
}