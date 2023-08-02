using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Attributes.DataSources;
using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;
using Amazon.CDK.AWS.Lambda;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SystemEnvironment = System.Environment;

namespace AllOverIt.Aws.Cdk.AppSync.Factories
{
    internal sealed class DataSourceFactory
    {
        private readonly IDictionary<string, BaseDataSource> _dataSourceCache = new Dictionary<string, BaseDataSource>();

        private readonly IGraphqlApi _graphQlApi;
        private readonly IReadOnlyDictionary<string, string> _endpointLookup;

        public DataSourceFactory(IGraphqlApi graphQlApi, IReadOnlyDictionary<string, string> endpointLookup)
        {
            _graphQlApi = graphQlApi.WhenNotNull(nameof(graphQlApi));
            _endpointLookup = endpointLookup ?? new Dictionary<string, string>();
        }

        public BaseDataSource CreateDataSource(DataSourceAttribute attribute)
        {
            var dataSourceId = GetDataSourceId(_graphQlApi.Node.Path, attribute.DataSourceName);

            if (!_dataSourceCache.TryGetValue(dataSourceId, out var dataSource))
            {
                dataSource = attribute switch
                {
                    LambdaDataSourceAttribute lambda => CreateLambdaDataSource(dataSourceId, lambda),
                    HttpDataSourceAttribute http => CreateHttpDataSource(dataSourceId, http),
                    NoneDataSourceAttribute none => CreateNoneDataSource(dataSourceId, "None", none),
                    SubscriptionDataSourceAttribute subscription => CreateNoneDataSource(dataSourceId, "Subscription", subscription),
                    _ => throw new ArgumentOutOfRangeException($"Unknown DataSource type '{attribute.GetType().Name}'")
                };

                _dataSourceCache.Add(dataSourceId, dataSource);
            }

            return dataSource;
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
            // exclude everything exception alphanumeric and dashes
            return Regex.Replace(value, @"[^\w]", "", RegexOptions.None);
        }

        private BaseDataSource CreateLambdaDataSource(string dataSourceId, LambdaDataSourceAttribute attribute)
        {
            var stack = Stack.Of(_graphQlApi);

            return new LambdaDataSource(stack, dataSourceId, new LambdaDataSourceProps
            {
                Api = _graphQlApi,
                Name = GetFullDataSourceName("Lambda", attribute.DataSourceName),
                Description = attribute.Description,
                LambdaFunction = Function.FromFunctionArn(stack, $"{attribute.FunctionName}Function",
                    $"arn:aws:lambda:{stack.Region}:{stack.Account}:function:{attribute.FunctionName}")
            });
        }

        private BaseDataSource CreateHttpDataSource(string dataSourceId, HttpDataSourceAttribute attribute)
        {
            var stack = Stack.Of(_graphQlApi);

            return new HttpDataSource(stack, dataSourceId, new HttpDataSourceProps
            {
                Api = _graphQlApi,
                Name = GetFullDataSourceName("Http", attribute.DataSourceName),
                Description = attribute.Description,
                Endpoint = GetHttpEndpoint(attribute.EndpointSource, attribute.EndpointKey)
            });
        }

        // Applicable to NoneDataSourceAttribute and SubscriptionDataSourceAttribute
        private BaseDataSource CreateNoneDataSource(string dataSourceId, string dataSourceNamePrefix, DataSourceAttribute attribute)
        {
            var stack = Stack.Of(_graphQlApi);

            return new NoneDataSource(stack, dataSourceId, new NoneDataSourceProps
            {
                Api = _graphQlApi,
                Name = GetFullDataSourceName(dataSourceNamePrefix, attribute.DataSourceName),
                Description = attribute.Description
            });
        }

        private string GetHttpEndpoint(EndpointSource endpointSource, string endpointKey)
        {
            return endpointSource switch
            {
                EndpointSource.Explicit => endpointKey,
                
                EndpointSource.ImportValue => Fn.ImportValue(endpointKey),
                
                EndpointSource.EnvironmentVariable => SystemEnvironment.GetEnvironmentVariable(endpointKey)
                    ?? throw new KeyNotFoundException($"Environment variable key '{endpointKey}' not found."),
                
                EndpointSource.Lookup => _endpointLookup.TryGetValue(endpointKey, out var lookupValue) 
                    ? lookupValue
                    : throw new KeyNotFoundException($"Lookup key '{endpointKey}' not found."),

                _ => throw new InvalidOperationException($"Unknown EndpointSource type '{endpointSource}'")
            };
        }
    }
}