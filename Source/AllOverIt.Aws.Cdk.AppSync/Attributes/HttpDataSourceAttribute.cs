using AllOverIt.Helpers;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    public sealed class HttpDataSourceAttribute : DataSourceAttribute
    {
        private readonly string _serviceName;

        public EndpointSource EndpointSource { get; }
        public string EndpointKey { get; }
        public override string LookupKey => SanitiseLookupKey($"{_serviceName}{EndpointKey}");

        public HttpDataSourceAttribute(string serviceName, string endpoint, string description = default)
            : this(serviceName, EndpointSource.Explicit, endpoint, description)
        {
        }

        public HttpDataSourceAttribute(string serviceName, EndpointSource endpointSource, string endpointKey, string description = default)
            : base(description)
        {
            _serviceName = serviceName.WhenNotNullOrEmpty(nameof(serviceName));
            EndpointSource = endpointSource;
            EndpointKey = endpointKey.WhenNotNullOrEmpty(nameof(endpointKey));
        }
    }
}