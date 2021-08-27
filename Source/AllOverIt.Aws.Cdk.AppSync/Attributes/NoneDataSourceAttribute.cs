using AllOverIt.Helpers;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    public class NoneDataSourceAttribute : DataSourceAttribute
    {
        private readonly string _serviceName;
        private readonly string _identifier;

        public override string LookupKey => SanitiseLookupKey($"{_serviceName}{_identifier}");

        public NoneDataSourceAttribute(string serviceName, string identifier, string description = default)
            : base(description)
        {
            _serviceName = serviceName.WhenNotNullOrEmpty(nameof(serviceName));
            _identifier = identifier.WhenNotNullOrEmpty(nameof(identifier));
        }
    }
}