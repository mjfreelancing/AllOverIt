using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Aws.Cdk.AppSync.Extensions;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.MappingTemplates;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using Amazon.CDK.AWS.AppSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync.Schema
{
    public sealed class SchemaBuilder : ISchemaBuilder
    {
        private const string QueryPrefix = "Query";
        private const string MutationPrefix = "Mutation";
        private const string SubscriptionPrefix = "Subscription";

        private readonly GraphqlApi _graphqlApi;
        private readonly IMappingTemplates _mappingTemplates;
        private readonly IGraphqlTypeStore _typeStore;
        private readonly IDataSourceFactory _dataSourceFactory;

        public SchemaBuilder(GraphqlApi graphQlApi, IMappingTemplates mappingTemplates, IGraphqlTypeStore typeStore, IDataSourceFactory dataSourceFactory)
        {
            _graphqlApi = graphQlApi.WhenNotNull(nameof(graphQlApi));
            _mappingTemplates = mappingTemplates.WhenNotNull(nameof(mappingTemplates));
            _typeStore = typeStore.WhenNotNull(nameof(typeStore));
            _dataSourceFactory = dataSourceFactory.WhenNotNull(nameof(dataSourceFactory));
        }

        public ISchemaBuilder AddQuery<TType>()
            where TType : IQueryDefinition
        {
            CreateGraphqlSchemaType<TType>((fieldName, field) => _graphqlApi.AddQuery(fieldName, field));
            return this;
        }

        public ISchemaBuilder AddMutation<TType>() where TType : IMutationDefinition
        {
            CreateGraphqlSchemaType<TType>((fieldName, field) => _graphqlApi.AddMutation(fieldName, field));
            return this;
        }

        public ISchemaBuilder AddSubscription<TType>() where TType : ISubscriptionDefinition
        {
            var schemaType = typeof(TType);

            var methods = schemaType.GetMethodInfo();

            if (schemaType.IsInterface)
            {
                var inheritedMethods = schemaType.GetInterfaces().SelectMany(item => item.GetMethods());
                methods = methods.Concat(inheritedMethods);
            }

            foreach (var methodInfo in methods)
            {
                var dataSource = methodInfo.GetDataSource(_dataSourceFactory);

                var isRequired = methodInfo.IsGqlTypeRequired();
                var isList = methodInfo.ReturnType.IsArray;
                var isRequiredList = isList && methodInfo.IsGqlArrayRequired();

                var returnObjectType = _typeStore
                    .GetGraphqlType(
                        null,
                        methodInfo.ReturnType,
                        methodInfo,
                        isRequired,
                        isList,
                        isRequiredList,
                        objectType => _graphqlApi.AddType(objectType));

                var mappingTemplateKey = $"{SubscriptionPrefix}.{methodInfo.Name}"; // methodInfo.GetFunctionName();

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
            var schemaType = typeof(TType);

            var methods = schemaType.GetMethodInfo();

            if (schemaType.IsInterface)
            {
                var inheritedMethods = schemaType.GetInterfaces().SelectMany(item => item.GetMethods());
                methods = methods.Concat(inheritedMethods);
            }

            foreach (var methodInfo in methods.Where(item => !item.IsSpecialName))
            {
                var dataSource = methodInfo.GetDataSource(_dataSourceFactory);

                if (dataSource == null)
                {
                    throw new SchemaException($"{schemaType.Name} is missing a required datasource for '{methodInfo.Name}'");
                }

                var isRequired = methodInfo.IsGqlTypeRequired();
                var isList = methodInfo.ReturnType.IsArray;
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
                        rootName,
                        methodInfo.ReturnType,
                        methodInfo,
                        isRequired,
                        isList,
                        isRequiredList,
                        objectType => _graphqlApi.AddType(objectType));

                var mappingTemplateKey = $"{rootName}.{methodInfo.Name}";   // methodInfo.GetFunctionName();

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