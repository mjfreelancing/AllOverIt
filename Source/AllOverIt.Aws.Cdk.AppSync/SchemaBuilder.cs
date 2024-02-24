using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Aws.Cdk.AppSync.Extensions;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using Amazon.CDK.AWS.AppSync;
using Cdklabs.AwsCdkAppsyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync
{
    internal sealed class SchemaBuilder
    {
        private const string QueryPrefix = "Query";
        private const string MutationPrefix = "Mutation";
        private const string SubscriptionPrefix = "Subscription";

        private readonly GraphqlApi _graphqlApi;
        private readonly GraphqlSchema _schema;
        private readonly MappingTemplates _mappingTemplates;
        private readonly MappingTypeFactory _mappingTypeFactory;
        private readonly GraphqlTypeStore _typeStore;
        private readonly DataSourceFactory _dataSourceFactory;

        public SchemaBuilder(GraphqlApi graphqlApi, GraphqlSchema schema, MappingTemplates mappingTemplates, MappingTypeFactory mappingTypeFactory, GraphqlTypeStore typeStore, DataSourceFactory dataSourceFactory)
        {
            _graphqlApi = graphqlApi.WhenNotNull();
            _schema = schema.WhenNotNull(nameof(schema));
            _mappingTemplates = mappingTemplates.WhenNotNull(nameof(mappingTemplates));
            _mappingTypeFactory = mappingTypeFactory.WhenNotNull(nameof(mappingTypeFactory));
            _typeStore = typeStore.WhenNotNull(nameof(typeStore));
            _dataSourceFactory = dataSourceFactory.WhenNotNull(nameof(dataSourceFactory));
        }

        public SchemaBuilder AddQuery<TType>()
            where TType : IQueryDefinition
        {
            CreateGraphqlSchemaType<TType>(
                (fieldName, field) => _schema.Query.AddField(new AddFieldOptions
                {
                    FieldName = fieldName,
                    Field = field
                }));

            return this;
        }

        public SchemaBuilder AddMutation<TType>() where TType : IMutationDefinition
        {
            CreateGraphqlSchemaType<TType>(
                (fieldName, field) => _schema.Mutation.AddField(new AddFieldOptions
                {
                    FieldName = fieldName,
                    Field = field
                }));

            return this;
        }

        public SchemaBuilder AddSubscription<TType>() where TType : ISubscriptionDefinition
        {
            SchemaUtils.AssertContainsNoProperties<TType>();

            var schemaType = typeof(TType);
            var methods = SchemaUtils.GetTypeMethods<TType>();

            foreach (var methodInfo in methods)
            {
                methodInfo.AssertReturnTypeIsNotNullable();

                var dataSource = methodInfo.GetDataSource(_dataSourceFactory);

                Throw<SchemaException>.WhenNull(dataSource, $"{schemaType.Name} is missing a required datasource for '{methodInfo.Name}'.");

                var fieldMapping = methodInfo.GetFieldName(SubscriptionPrefix);

                methodInfo.RegisterRequestResponseMappings(fieldMapping, _mappingTemplates, _mappingTypeFactory);

                var requiredTypeInfo = methodInfo.GetRequiredTypeInfo();

                var returnObjectType = _typeStore
                    .GetGraphqlType(
                        fieldMapping,
                        requiredTypeInfo,
                        objectType => _schema.AddType(objectType));

                var fieldName = methodInfo.Name.GetGraphqlName();

                _schema.Subscription.AddField(new AddFieldOptions
                {
                    FieldName = fieldName,
                    Field = new Field(
                        new FieldOptions
                        {
                            Directives = [
                                Directive.Subscribe(GetSubscriptionMutations(methodInfo).ToArray())
                            ],
                            Args = methodInfo.GetMethodArgs(_schema, _typeStore),
                            ReturnType = returnObjectType
                        })
                });

                if (dataSource is not null)
                {
                    CreateResolver(SubscriptionPrefix, fieldName, dataSource, fieldMapping);
                }
            }

            return this;
        }

        private void CreateGraphqlSchemaType<TType>(Action<string, Field> graphqlAction)
        {
            SchemaUtils.AssertContainsNoProperties<TType>();

            var schemaType = typeof(TType);
            var methods = SchemaUtils.GetTypeMethods<TType>();

            foreach (var methodInfo in methods)
            {
                methodInfo.AssertReturnTypeIsNotNullable();

                var dataSource = methodInfo.GetDataSource(_dataSourceFactory);

                Throw<SchemaException>.WhenNull(dataSource, $"{schemaType.Name} is missing a required datasource for '{methodInfo.Name}'.");

                string rootName;

                if (typeof(IQueryDefinition).IsAssignableFrom(schemaType))
                {
                    rootName = QueryPrefix;
                }
                else if (typeof(IMutationDefinition).IsAssignableFrom(schemaType))
                {
                    rootName = MutationPrefix;
                }
                else
                {
                    throw new InvalidOperationException($"Expected a {nameof(IQueryDefinition)} or {nameof(IMutationDefinition)} type");
                }

                var fieldMapping = methodInfo.GetFieldName(rootName);

                methodInfo.RegisterRequestResponseMappings(fieldMapping, _mappingTemplates, _mappingTypeFactory);

                var requiredTypeInfo = methodInfo.GetRequiredTypeInfo();

                var returnObjectType = _typeStore
                    .GetGraphqlType(
                        fieldMapping,
                        requiredTypeInfo,
                        objectType => _schema.AddType(objectType));

                var authDirectives = methodInfo.GetAuthDirectivesOrDefault();

                // AddQuery / AddMutation
                var fieldName = methodInfo.Name.GetGraphqlName();

                graphqlAction.Invoke(fieldName,
                    new Field(
                        new FieldOptions
                        {
                            Args = methodInfo.GetMethodArgs(_schema, _typeStore),
                            ReturnType = returnObjectType,
                            Directives = authDirectives
                        })
                );

                if (dataSource is not null)
                {
                    CreateResolver(rootName, fieldName, dataSource, fieldMapping);
                }
            }
        }

        private static IEnumerable<string> GetSubscriptionMutations(MethodInfo methodInfo)
        {
            var attribute = methodInfo.GetCustomAttribute<SubscriptionMutationAttribute>(true);

            return attribute == null
                ? []
                : attribute!.Mutations;
        }

        private void CreateResolver(string parentName, string fieldName, BaseDataSource dataSource, string fieldMapping)
        {
            var mappingType = _mappingTemplates.GetMappingType(fieldMapping);
            var resolverProps = new ExtendedResolverProps
            {
                TypeName = parentName,
                FieldName = fieldName,
                DataSource = dataSource
            };

            if (mappingType == MappingType.Code)
            {
                resolverProps.Code = _mappingTemplates.GetCodeMapping(fieldMapping);
            }
            else
            {
                resolverProps.RequestMappingTemplate = _mappingTemplates.GetRequestMapping(fieldMapping);
                resolverProps.ResponseMappingTemplate = _mappingTemplates.GetResponseMapping(fieldMapping);
            }

            _graphqlApi.CreateResolver($"{parentName}{fieldName}Resolver", resolverProps);
        }
    }
}