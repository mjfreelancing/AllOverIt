using AllOverIt.Assertion;

namespace AllOverIt.Aws.Cdk.AppSync.DataSources
{
    /// <summary>Provides information required to declare an AppSync subscription DataSource.</summary>
    public class SubscriptionGraphQlDataSource : GraphQlDataSourceBase
    {
        private readonly string _identifier;

        /// <inheritdoc />
        public override string DataSourceName => _identifier;

        /// <summary>Constructor.</summary>
        /// <param name="identifier">A unique identifier for this datasource.</param>
        /// <param name="description">A description for the datasource.</param>
        public SubscriptionGraphQlDataSource(string identifier, string description = default)
            : base(description)
        {
            _identifier = identifier.WhenNotNullOrEmpty(nameof(identifier));
        }
    }
}