using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.DataSources
{
    /// <summary>Provides information required to declare an AppSync HTTP datasource.</summary>
    public class HttpDataSourceAttribute : DataSourceAttribute
    {
        /// <summary>Determines how the <see cref="EndpointKey"/> value should be interpreted.</summary>
        public EndpointSource EndpointSource { get; }

        /// <summary>This value is interpreted based on the <see cref="EndpointSource"/>.</summary>
        public string EndpointKey { get; }

        /// <inheritdoc />
        public override string DataSourceName => EndpointKey;

        /// <summary>Constructor.</summary>
        /// <param name="endpoint">An explicit HTTP endpoint. <see cref="EndpointSource"/> is set to 'Explicit' (<see cref="EndpointSource"/>).</param>
        /// <param name="mappingType">The mapping type, inheriting <see cref="IRequestResponseMapping"/>, that provides the datasource request and response
        /// mapping. If null, it is expected that the required mapping will be provided via <see cref="MappingTemplates"/>.</param>
        /// <param name="description">A description for the datasource.</param>
        public HttpDataSourceAttribute(string endpoint, SystemType mappingType = default, string description = default)
            : this(EndpointSource.Explicit, endpoint, mappingType, description)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="endpointSource">Determines how the <see cref="EndpointKey"/> value should be interpreted.</param>
        /// <param name="endpointKey">This value is interpreted based on the <see cref="EndpointSource"/>.</param>
        /// <param name="mappingType">The mapping type, inheriting <see cref="IRequestResponseMapping"/>, that provides the datasource request and response
        /// mapping. If null, it is expected that the required mapping will be provided via <see cref="MappingTemplates"/>.</param>
        /// <param name="description">A description for the datasource.</param>
        public HttpDataSourceAttribute(EndpointSource endpointSource, string endpointKey, SystemType mappingType = default, string description = default)
            : base(mappingType, description)
        {
            EndpointSource = endpointSource;
            EndpointKey = endpointKey.WhenNotNullOrEmpty(nameof(endpointKey));
        }
    }
}