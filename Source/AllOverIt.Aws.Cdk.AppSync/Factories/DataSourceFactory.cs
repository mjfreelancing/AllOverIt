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
        private readonly Dictionary<string, BaseDataSource> _dataSourceCache = [];

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
                    LambdaDataSourceAttribute lambda => CreateLambdaDataSource(dataSourceId, lambda.FunctionName, lambda.Description),
                    HttpDataSourceAttribute http => CreateHttpDataSource(dataSourceId, http.DataSourceName, http.EndpointSource, http.EndpointKey, http.Description),
                    NoneDataSourceAttribute none => CreateNoneDataSource(dataSourceId, "None", none.DataSourceName, none.Description),
                    SubscriptionDataSourceAttribute subscription => CreateNoneDataSource(dataSourceId, "Subscription", subscription.DataSourceName, subscription.Description),
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
            // exclude everything except alphanumeric and dashes
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

        private HttpDataSource CreateHttpDataSource(string dataSourceId, string datasourceName, EndpointSource endpointSource, string endpointKey, string description)
        {
            var stack = Stack.Of(_graphQlApi);

            return new HttpDataSource(stack, dataSourceId, new HttpDataSourceProps
            {
                Api = _graphQlApi,
                Name = GetFullDataSourceName("Http", datasourceName),
                Description = description,
                Endpoint = GetHttpEndpoint(endpointSource, endpointKey)
            });
        }

        // Applicable to NoneDataSourceAttribute and SubscriptionDataSourceAttribute
        private NoneDataSource CreateNoneDataSource(string dataSourceId, string dataSourceNamePrefix, string dataSourceName, string description)
        {
            var stack = Stack.Of(_graphQlApi);

            return new NoneDataSource(stack, dataSourceId, new NoneDataSourceProps
            {
                Api = _graphQlApi,
                Name = GetFullDataSourceName(dataSourceNamePrefix, dataSourceName),
                Description = description
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