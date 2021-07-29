using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Aws.Cdk.AppSync.Extensions;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using AllOverIt.Helpers;
using Amazon.CDK.AWS.AppSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync
{
    public sealed class SchemaBuilder
    {
        private const string QueryPrefix = "Query";
        private const string MutationPrefix = "Mutation";
        private const string SubscriptionPrefix = "Subscription";

        private readonly GraphqlApi _graphqlApi;
        private readonly MappingTemplatesBase _mappingTemplates;
        private readonly GraphqlTypeStore _typeStore;
        private readonly DataSourceFactory _dataSourceFactory;

        public SchemaBuilder(GraphqlApi graphQlApi, MappingTemplatesBase mappingTemplates, GraphqlTypeStore typeStore, DataSourceFactory dataSourceFactory)
        {
            _graphqlApi = graphQlApi.WhenNotNull(nameof(graphQlApi));
            _mappingTemplates = mappingTemplates.WhenNotNull(nameof(mappingTemplates));
            _typeStore = typeStore.WhenNotNull(nameof(typeStore));
            _dataSourceFactory = dataSourceFactory.WhenNotNull(nameof(dataSourceFactory));
        }

        public SchemaBuilder AddQuery<TType>()
            where TType : IQueryDefinition
        {
            CreateGraphqlSchemaType<TType>((fieldName, field) => _graphqlApi.AddQuery(fieldName, field));
            return this;
        }

        public SchemaBuilder AddMutation<TType>() where TType : IMutationDefinition
        {
            CreateGraphqlSchemaType<TType>((fieldName, field) => _graphqlApi.AddMutation(fieldName, field));
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

                if (dataSource == null)
                {
                    throw new SchemaException($"{schemaType.Name} is missing a required datasource for '{methodInfo.Name}'");
                }

                var isRequired = methodInfo.IsGqlTypeRequired();
                var isList = methodInfo.ReturnType.IsArray;
                var isRequiredList = isList && methodInfo.IsGqlArrayRequired();

                var returnObjectType = _typeStore
                    .GetGraphqlType(
                        methodInfo.GetFieldName(SubscriptionPrefix),
                        methodInfo.ReturnType,
                        isRequired,
                        isList,
                        isRequiredList,
                        objectType => _graphqlApi.AddType(objectType));

                var mappingTemplateKey = $"{SubscriptionPrefix}.{methodInfo.Name}";

                _graphqlApi.AddSubscription(methodInfo.Name.GetGraphqlName(),
                    new ResolvableField(
                        new ResolvableFieldOptions
                        {
                            DataSource = dataSource,
                            RequestMappingTemplate = MappingTemplate.FromString(_mappingTemplates.GetRequestMapping(mappingTemplateKey)),
                            ResponseMappingTemplate = MappingTemplate.FromString(_mappingTemplates.GetResponseMapping(mappingTemplateKey)),
                            Directives = new[]
                            {
                                Directive.Subscribe(GetSubscriptionMutations(methodInfo).ToArray())
                            },
                            Args = methodInfo.GetMethodArgs(_graphqlApi, _typeStore),
                            ReturnType = returnObjectType
                        })
                );
            }

            return this;
        }

        private void CreateGraphqlSchemaType<TType>(Action<string, ResolvableField> graphqlAction)
        {
            SchemaUtils.AssertContainsNoProperties<TType>();

            var schemaType = typeof(TType);
            var methods = SchemaUtils.GetTypeMethods<TType>();

            foreach (var methodInfo in methods)
            {
                methodInfo.AssertReturnTypeIsNotNullable();

                var dataSource = methodInfo.GetDataSource(_dataSourceFactory);

                if (dataSource == null)
                {
                    throw new SchemaException($"{schemaType.Name} is missing a required datasource for '{methodInfo.Name}'");
                }

                var returnType = methodInfo.ReturnType;
                var isRequired = methodInfo.IsGqlTypeRequired();
                var isList = returnType.IsArray;
                var isRequiredList = isList && methodInfo.IsGqlArrayRequired();

                string rootName = null;

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
                    throw new InvalidOperationException($"Expected a {typeof(IQueryDefinition).Name} or {typeof(ISubscriptionDefinition).Name} type");
                }

                var returnObjectType = _typeStore
                    .GetGraphqlType(
                        methodInfo.GetFieldName(rootName),
                        returnType,
                        isRequired,
                        isList,
                        isRequiredList,
                        objectType => _graphqlApi.AddType(objectType));

                var mappingTemplateKey = $"{rootName}.{methodInfo.Name}";

                graphqlAction.Invoke(methodInfo.Name.GetGraphqlName(),
                    new ResolvableField(
                        new ResolvableFieldOptions
                        {
                            DataSource = dataSource,
                            RequestMappingTemplate = MappingTemplate.FromString(_mappingTemplates.GetRequestMapping(mappingTemplateKey)),
                            ResponseMappingTemplate = MappingTemplate.FromString(_mappingTemplates.GetResponseMapping(mappingTemplateKey)),
                            Args = methodInfo.GetMethodArgs(_graphqlApi, _typeStore),
                            ReturnType = returnObjectType
                        })
                );
            }
        }

        private static IEnumerable<string> GetSubscriptionMutations(MethodInfo methodInfo)
        {
            var attribute = methodInfo.GetCustomAttribute<SubscriptionMutationAttribute>(true);

            return attribute == null
                ? Enumerable.Empty<string>()
                : attribute!.Mutations;
        }
    }
}