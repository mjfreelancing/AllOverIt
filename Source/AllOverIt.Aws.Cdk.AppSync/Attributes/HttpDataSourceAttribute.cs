using AllOverIt.Helpers;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    public class HttpDataSourceAttribute : DataSourceAttribute
    {
        private readonly string _serviceName;

        public EndpointSource EndpointSource { get; }
        public string EndpointKey { get; }
        public override string LookupKey => SanitiseLookupKey($"{_serviceName}{EndpointKey}");

        public HttpDataSourceAttribute(string serviceName, string endpoint, SystemType mappingType = default, string description = default)
            : this(serviceName, EndpointSource.Explicit, endpoint, mappingType, description)
        {
        }

        public HttpDataSourceAttribute(string serviceName, EndpointSource endpointSource, string endpointKey, SystemType mappingType = default, string description = default)
            : base(mappingType, description)
        {
            _serviceName = serviceName.WhenNotNullOrEmpty(nameof(serviceName));
            EndpointSource = endpointSource;
            EndpointKey = endpointKey.WhenNotNullOrEmpty(nameof(endpointKey));
        }
    }
}