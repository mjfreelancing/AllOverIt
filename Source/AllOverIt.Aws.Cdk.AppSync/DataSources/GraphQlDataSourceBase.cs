namespace AllOverIt.Aws.Cdk.AppSync.DataSources
{
    /// <summary>An abstract base class for all DataSource types.</summary>
    public abstract class GraphQlDataSourceBase
    {
        /// <summary>The DataSource name.</summary>
        public abstract string DataSourceName { get; }

        /// <summary>A description for the DataSource.</summary>
        public string Description { get; }

        protected GraphQlDataSourceBase(string description)
        {
            Description = description;
        }
    }
}