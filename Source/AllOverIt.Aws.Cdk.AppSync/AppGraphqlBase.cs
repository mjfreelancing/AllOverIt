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
        /// <param name="mappingTemplates">Contains request and response mapping templates for all datasources not decorated
        /// with a [RequestResponseMapping] attribute. If an instance is not provided then an internal version will be created
        /// and there is an assumption that all mappings are provided via attributes.</param>
        protected AppGraphqlBase(Construct scope, string id, IGraphqlApiProps apiProps, MappingTemplates mappingTemplates = null)
            : base(scope, id, apiProps)
        {
            mappingTemplates ??= new MappingTemplates();

            var dataSourceFactory = new DataSourceFactory(this);
            var gqlTypeCache = new GraphqlTypeStore(this, mappingTemplates, dataSourceFactory);
            _schemaBuilder = new SchemaBuilder(this, mappingTemplates, gqlTypeCache, dataSourceFactory);
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