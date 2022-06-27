﻿using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Aws.Cdk.AppSync.Extensions;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using Amazon.CDK.AWS.AppSync;
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
        private readonly MappingTemplates _mappingTemplates;
        private readonly MappingTypeFactory _mappingTypeFactory;
        private readonly GraphqlTypeStore _typeStore;
        private readonly DataSourceFactory _dataSourceFactory;

        public SchemaBuilder(GraphqlApi graphQlApi, MappingTemplates mappingTemplates, MappingTypeFactory mappingTypeFactory, GraphqlTypeStore typeStore, DataSourceFactory dataSourceFactory)
        {
            _graphqlApi = graphQlApi.WhenNotNull(nameof(graphQlApi));
            _mappingTemplates = mappingTemplates.WhenNotNull(nameof(mappingTemplates));
            _mappingTypeFactory = mappingTypeFactory.WhenNotNull(nameof(mappingTypeFactory));
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

                var fieldMapping = methodInfo.GetFieldName(SubscriptionPrefix);

                methodInfo.RegisterRequestResponseMappings(fieldMapping, _mappingTemplates, _mappingTypeFactory);

                var requiredTypeInfo = methodInfo.GetRequiredTypeInfo();

                var returnObjectType = _typeStore
                    .GetGraphqlType(
                        fieldMapping,
                        requiredTypeInfo,
                        objectType => _graphqlApi.AddType(objectType));

                _graphqlApi.AddSubscription(methodInfo.Name.GetGraphqlName(),
                    new ResolvableField(
                        new ResolvableFieldOptions
                        {
                            DataSource = dataSource,
                            RequestMappingTemplate = _mappingTemplates.GetRequestMapping(fieldMapping),
                            ResponseMappingTemplate = _mappingTemplates.GetResponseMapping(fieldMapping),
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
                    throw new InvalidOperationException($"Expected a {nameof(IQueryDefinition)} or {nameof(ISubscriptionDefinition)} type");
                }

                var fieldMapping = methodInfo.GetFieldName(rootName);

                methodInfo.RegisterRequestResponseMappings(fieldMapping, _mappingTemplates, _mappingTypeFactory);

                var requiredTypeInfo = methodInfo.GetRequiredTypeInfo();

                var returnObjectType = _typeStore
                    .GetGraphqlType(
                        fieldMapping,
                        requiredTypeInfo,
                        objectType => _graphqlApi.AddType(objectType));

                var authDirectives = methodInfo.GetAuthDirectivesOrDefault();

                // AddQuery / AddMutation
                graphqlAction.Invoke(methodInfo.Name.GetGraphqlName(),
                    new ResolvableField(
                        new ResolvableFieldOptions
                        {
                            DataSource = dataSource,
                            RequestMappingTemplate = _mappingTemplates.GetRequestMapping(fieldMapping),
                            ResponseMappingTemplate = _mappingTemplates.GetResponseMapping(fieldMapping),
                            Args = methodInfo.GetMethodArgs(_graphqlApi, _typeStore),
                            ReturnType = returnObjectType,
                            Directives = authDirectives
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