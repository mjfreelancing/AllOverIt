using AllOverIt.Assertion;

namespace AllOverIt.Aws.Cdk.AppSync.DataSources
{
    /// <summary>Provides information required to declare an AppSync HTTP DataSource.</summary>
    public sealed class HttpGraphqlDataSource : GraphqlDataSourceBase
    {
        /// <summary>The HTTP endpoint.</summary>
        public string Endpoint { get; }

        /// <summary>Constructor.</summary>
        /// <param name="dataSourceName">The DataSource name.</param>
        /// <param name="endpoint">The HTTP endpoint.</param>
        /// <param name="description">A description for the datasource.</param>
        public HttpGraphqlDataSource(string dataSourceName, string endpoint, string? description = default)
            : base(dataSourceName, description)
        {
            Endpoint = endpoint.WhenNotNullOrEmpty();
        }
    }
}