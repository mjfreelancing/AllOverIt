using AllOverIt.Assertion;

namespace AllOverIt.Aws.Cdk.AppSync.DataSources
{
    /// <summary>An abstract base class for all DataSource types.</summary>
    public abstract class GraphqlDataSourceBase
    {
        /// <summary>The DataSource name.</summary>
        public string DataSourceName { get; }

        /// <summary>A description for the DataSource.</summary>
        public string? Description { get; }

        /// <summary>Constructor.</summary>
        /// <param name="dataSourceName">The DataSource name.</param>
        /// <param name="description">A description for the DataSource.</param>
        protected GraphqlDataSourceBase(string dataSourceName, string? description)
        {
            DataSourceName = dataSourceName.WhenNotNullOrEmpty(nameof(dataSourceName));
            Description = description;
        }
    }
}