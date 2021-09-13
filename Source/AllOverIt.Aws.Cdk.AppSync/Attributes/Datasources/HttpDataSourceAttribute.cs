using AllOverIt.Helpers;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Datasources
{
    public class HttpDataSourceAttribute : DataSourceAttribute
    {
        public EndpointSource EndpointSource { get; }
        public string EndpointKey { get; }
        public override string DataSourceName => EndpointKey;

        public HttpDataSourceAttribute(string endpoint, SystemType mappingType = default, string description = default)
            : this(EndpointSource.Explicit, endpoint, mappingType, description)
        {
        }

        public HttpDataSourceAttribute(EndpointSource endpointSource, string endpointKey, SystemType mappingType = default,
            string description = default)
            : base(mappingType, description)
        {
            EndpointSource = endpointSource;
            EndpointKey = endpointKey.WhenNotNullOrEmpty(nameof(endpointKey));
        }
    }
}