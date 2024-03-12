﻿using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers;
using AllOverIt.Aws.Cdk.AppSync.DataSources;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;
using Amazon.CDK.AWS.Lambda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AllOverIt.Aws.Cdk.AppSync.Factories
{
    internal sealed class DataSourceFactory
    {
        private readonly Dictionary<string, BaseDataSource> _dataSourceCache = [];

        private readonly IGraphqlApi _graphQlApi;
        private readonly Dictionary<string, GraphQlDataSourceBase> _dataSources;

        public DataSourceFactory(IGraphqlApi graphQlApi, IReadOnlyCollection<GraphQlDataSourceBase> dataSources)
        {
            _graphQlApi = graphQlApi.WhenNotNull(nameof(graphQlApi));

            _dataSources = dataSources
                .WhenNotNull(nameof(dataSources))
                .Select(dataSource => new KeyValuePair<string, GraphQlDataSourceBase>(dataSource.DataSourceName, dataSource))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public BaseDataSource CreateDataSource(GraphQlResolverAttribute attribute)
        {
            if (attribute is UnitResolverAttribute unitResolverAttribute)
            {
                var datasourceLookup = unitResolverAttribute.DataSourceName;

                if (!_dataSources.TryGetValue(datasourceLookup, out var graphqlDataSource))
                {
                    Throw<SchemaException>.WhenNull(graphqlDataSource, $"Unknown DataSource Id: '{datasourceLookup}'");
                }

                var dataSourceId = GetDataSourceId(_graphQlApi.Node.Path, graphqlDataSource.DataSourceName);

                if (!_dataSourceCache.TryGetValue(dataSourceId, out var dataSource))
                {
                    dataSource = graphqlDataSource switch
                    {
                        LambdaGraphQlDataSource lambda => CreateLambdaDataSource(dataSourceId, lambda.FunctionName, lambda.Description),
                        HttpGraphQlDataSource http => CreateHttpDataSource(dataSourceId, http.DataSourceName, http.Endpoint, http.Description),
                        NoneGraphQlDataSource none => CreateNoneDataSource(dataSourceId, none.DataSourceName, none.Description),
                        _ => throw new ArgumentOutOfRangeException($"Unknown DataSource type '{attribute.GetType().Name}'")
                    };

                    _dataSourceCache.Add(dataSourceId, dataSource);
                }

                return dataSource;
            }
            //else if (attribute is PipelineResolverAttribute pipelineResolverAttribute)
            //{
            //    Pipelines yet to be implemented
            //}

            return null;
        }

        private static string GetDataSourceId(string nodePath, string dataSourceName)
        {
            return SanitizeValue($"{nodePath}{dataSourceName}");
        }

        private static string GetFullDataSourceName(string dataSourcePrefix, string value)
        {
            return SanitizeValue($"{value}{dataSourcePrefix}DataSource");
        }

        private static string SanitizeValue(string value)
        {
            // Exclude everything except alphanumeric and dashes.
            return Regex.Replace(value, @"[^\w]", "", RegexOptions.None);
        }

        private LambdaDataSource CreateLambdaDataSource(string dataSourceId, string functionName, string description)
        {
            var stack = Stack.Of(_graphQlApi);

            return new LambdaDataSource(stack, dataSourceId, new LambdaDataSourceProps
            {
                Api = _graphQlApi,
                Name = GetFullDataSourceName("Lambda", functionName),       // Using functionName as the DataSourceName
                Description = description,
                LambdaFunction = Function.FromFunctionArn(stack, $"{functionName}Function",
                    $"arn:aws:lambda:{stack.Region}:{stack.Account}:function:{functionName}")
            });
        }

        private HttpDataSource CreateHttpDataSource(string dataSourceId, string datasourceName, string endpoint, string description)
        {
            var stack = Stack.Of(_graphQlApi);

            return new HttpDataSource(stack, dataSourceId, new HttpDataSourceProps
            {
                Api = _graphQlApi,
                Name = GetFullDataSourceName("Http", datasourceName),
                Description = description,
                Endpoint = endpoint
            });
        }

        // Applicable to NONE and Subscription DataSources
        private NoneDataSource CreateNoneDataSource(string dataSourceId, string dataSourceName, string description)
        {
            var stack = Stack.Of(_graphQlApi);

            return new NoneDataSource(stack, dataSourceId, new NoneDataSourceProps
            {
                Api = _graphQlApi,
                Name = GetFullDataSourceName("None", dataSourceName),
                Description = description
            });
        }
    }
}