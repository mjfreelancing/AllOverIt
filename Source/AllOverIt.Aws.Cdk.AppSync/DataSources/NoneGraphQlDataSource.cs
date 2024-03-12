namespace AllOverIt.Aws.Cdk.AppSync.DataSources
{
    /// <summary>Provides information required to declare an AppSync NONE DataSource.</summary>
    public class NoneGraphQlDataSource : GraphQlDataSourceBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="dataSourceName">The DataSource name.</param>
        /// <param name="description">A description for the datasource.</param>
        public NoneGraphQlDataSource(string dataSourceName, string description = default)
            : base(dataSourceName, description)
        {
        }
    }
}