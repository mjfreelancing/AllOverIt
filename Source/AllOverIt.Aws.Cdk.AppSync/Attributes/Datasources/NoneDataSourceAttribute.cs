using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Helpers;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.DataSources
{
    /// <summary>Provides information required to declare an AppSync None datasource.</summary>
    public class NoneDataSourceAttribute : DataSourceAttribute
    {
        private readonly string _identifier;

        /// <inheritdoc />
        public override string DataSourceName => _identifier;

        /// <summary>Constructor.</summary>
        /// <param name="identifier">A unique identifier for this datasource.</param>
        /// <param name="mappingType">The mapping type, inheriting <see cref="IRequestResponseMapping"/>, that provides the datasource request and response
        /// mapping. If null, it is expected that the required mapping will be provided via <see cref="MappingTemplates"/>.</param>
        /// <param name="description">A description for the datasource.</param>
        public NoneDataSourceAttribute(string identifier, SystemType mappingType = default, string description = default)
            : base(mappingType, description)
        {
            _identifier = identifier.WhenNotNullOrEmpty(nameof(identifier));
        }
    }
}