namespace AllOverIt.Aws.Cdk.AppSync.DataSources
{
    /// <summary>An abstract base class for all DataSource types.</summary>
    public abstract class GraphQlDataSourceBase
    {
        /// <summary>The DataSource name.</summary>
        public string DataSourceName { get; }

        /// <summary>A description for the DataSource.</summary>
        public string Description { get; }

        protected GraphQlDataSourceBase(string dataSourceName, string description)
        {
            DataSourceName = dataSourceName;
            Description = description;
        }
    }
}