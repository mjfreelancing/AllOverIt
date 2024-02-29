using AllOverIt.Assertion;

namespace AllOverIt.Aws.Cdk.AppSync.DataSources
{
    /// <summary>Provides information required to declare an AppSync HTTP DataSource.</summary>
    public sealed class HttpGraphQlDataSource : GraphQlDataSourceBase
    {
        /// <summary>Determines how the <see cref="EndpointKey"/> value should be interpreted.</summary>
        public EndpointSource EndpointSource { get; }

        /// <summary>This value is interpreted based on the <see cref="EndpointSource"/>.</summary>
        public string EndpointKey { get; }

        /// <inheritdoc />
        public override string DataSourceName => EndpointKey;

        /// <summary>Constructor.</summary>
        /// <param name="endpoint">An explicit HTTP endpoint. <see cref="EndpointSource"/> is set to 'Explicit' (<see cref="EndpointSource"/>).</param>
        /// <param name="description">A description for the datasource.</param>
        public HttpGraphQlDataSource(string endpoint, string description = default)
            : this(EndpointSource.Explicit, endpoint, description)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="endpointSource">Determines how the <see cref="EndpointKey"/> value should be interpreted.</param>
        /// <param name="endpointKey">This value is interpreted based on the <see cref="EndpointSource"/>.</param>
        /// <param name="description">A description for the datasource.</param>
        public HttpGraphQlDataSource(EndpointSource endpointSource, string endpointKey, string description = default)
            : base(description)
        {
            EndpointSource = endpointSource;
            EndpointKey = endpointKey.WhenNotNullOrEmpty(nameof(endpointKey));
        }
    }
}