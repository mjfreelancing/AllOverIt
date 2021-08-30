using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Helpers;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;
using Amazon.CDK.AWS.Lambda;
using System;
using System.Collections.Generic;
using SystemEnvironment = System.Environment;

namespace AllOverIt.Aws.Cdk.AppSync.Factories
{
    public sealed class DataSourceFactory
    {
        private readonly IDictionary<string, BaseDataSource> _dataSourceCache = new Dictionary<string, BaseDataSource>();

        private readonly IGraphqlApi _graphQlApi;

        public DataSourceFactory(IGraphqlApi graphQlApi)
        {
            _graphQlApi = graphQlApi.WhenNotNull(nameof(graphQlApi));
        }

        public BaseDataSource CreateDataSource(DataSourceAttribute attribute)
        {
            if (!_dataSourceCache.TryGetValue(attribute.LookupKey, out var dataSource))
            {
                dataSource = attribute switch
                {
                    LambdaDataSourceAttribute lambdaDataSourceAttribute => CreateLambdaDataSource(lambdaDataSourceAttribute),
                    HttpDataSourceAttribute httpDataSourceAttribute => CreateHttpDataSource(httpDataSourceAttribute),
                    NoneDataSourceAttribute noneDataSourceAttribute => CreateNoneDataSource(noneDataSourceAttribute),
                    _ => throw new InvalidOperationException($"Unhandled DataSource type '{attribute.GetType().Name}'")
                };

                _dataSourceCache.Add(attribute.LookupKey, dataSource);
            }

            return dataSource;
        }

        private BaseDataSource CreateLambdaDataSource(LambdaDataSourceAttribute attribute)
        {
            var stack = Stack.Of(_graphQlApi);

            return new LambdaDataSource(stack, $"{attribute.LookupKey}DataSource", new LambdaDataSourceProps
            {
                Api = _graphQlApi,
                Name = $"{attribute.LookupKey}DataSource",
                Description = attribute.Description,
                LambdaFunction = Function.FromFunctionArn(stack, $"{attribute.FunctionName}Function",
                    $"arn:aws:lambda:{stack.Region}:{stack.Account}:function:{attribute.FunctionName}")
            });
        }

        private BaseDataSource CreateHttpDataSource(HttpDataSourceAttribute attribute)
        {
            var stack = Stack.Of(_graphQlApi);

            return new HttpDataSource(stack, $"{attribute.LookupKey}DataSource", new HttpDataSourceProps
            {
                Api = _graphQlApi,
                Name = $"{attribute.LookupKey}DataSource",
                Description = attribute.Description,
                Endpoint = GetHttpEndpoint(attribute.EndpointSource, attribute.EndpointKey)
            });
        }

        private BaseDataSource CreateNoneDataSource(NoneDataSourceAttribute attribute)
        {
            var stack = Stack.Of(_graphQlApi);

            return new NoneDataSource(stack, $"{attribute.LookupKey}DataSource", new NoneDataSourceProps
            {
                Api = _graphQlApi,
                Name = $"{attribute.LookupKey}DataSource",
                Description = attribute.Description
            });
        }

        private static string GetHttpEndpoint(EndpointSource endpointSource, string endpointKey)
        {
            return endpointSource switch
            {
                EndpointSource.Explicit => endpointKey,
                EndpointSource.ImportValue => Fn.ImportValue(endpointKey),
                EndpointSource.EnvironmentVariable => SystemEnvironment.GetEnvironmentVariable(endpointKey) ?? string.Empty,
                _ => throw new InvalidOperationException($"Unknown EndpointSource type '{endpointSource}'")
            };
        }
    }
}