using AllOverIt.Assertion;

namespace AllOverIt.Aws.Cdk.AppSync.DataSources
{
    /// <summary>Provides information required to declare an AppSync NONE DataSource.</summary>
    public class NoneGraphQlDataSource : GraphQlDataSourceBase
    {
        private readonly string _identifier;

        /// <inheritdoc />
        public override string DataSourceName => _identifier;

        /// <summary>Constructor.</summary>
        /// <param name="identifier">A unique identifier for this datasource.</param>
        /// <param name="description">A description for the datasource.</param>
        public NoneGraphQlDataSource(string identifier, string description = default)
            : base(description)
        {
            _identifier = identifier.WhenNotNullOrEmpty(nameof(identifier));
        }
    }
}