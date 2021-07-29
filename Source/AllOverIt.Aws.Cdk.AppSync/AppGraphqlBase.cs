﻿using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;

namespace AllOverIt.Aws.Cdk.AppSync
{
    public abstract class AppGraphqlBase : GraphqlApi
    {
        private readonly SchemaBuilder _schemaBuilder;

        public AppGraphqlBase(Construct scope, string id, IGraphqlApiProps apiProps, MappingTemplatesBase mappingTemplates)
            : base(scope, id, apiProps)
        {
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