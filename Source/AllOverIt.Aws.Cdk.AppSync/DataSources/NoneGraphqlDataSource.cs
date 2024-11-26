namespace AllOverIt.Aws.Cdk.AppSync.DataSources
{
    /// <summary>Provides information required to declare an AppSync NONE DataSource.</summary>
    public class NoneGraphqlDataSource : GraphqlDataSourceBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="dataSourceName">The DataSource name.</param>
        /// <param name="description">A description for the datasource.</param>
        public NoneGraphqlDataSource(string dataSourceName, string? description = default)
            : base(dataSourceName, description)
        {
        }
    }
}