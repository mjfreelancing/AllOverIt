using AllOverIt.Assertion;

namespace AllOverIt.Aws.Cdk.AppSync.DataSources
{
    /// <summary>Provides information required to declare an AppSync EventBridge DataSource.</summary>
    public sealed class EventBridgeGraphqlDataSource : GraphqlDataSourceBase
    {
        /// <summary>The event bus name.</summary>
        public string EventBusName { get; }

        /// <summary>Constructor.</summary>
        /// <param name="dataSourceName">The DataSource name.</param>
        /// <param name="eventBusName">The event bus name.</param>
        /// <param name="description">A description for the datasource.</param>
        public EventBridgeGraphqlDataSource(string dataSourceName, string eventBusName, string? description = default)
            : base(dataSourceName, description)
        {
            EventBusName = eventBusName.WhenNotNullOrEmpty();
        }
    }
}