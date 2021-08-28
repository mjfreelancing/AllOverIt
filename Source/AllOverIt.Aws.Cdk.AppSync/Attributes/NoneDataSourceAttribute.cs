using AllOverIt.Helpers;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    public class NoneDataSourceAttribute : DataSourceAttribute
    {
        private readonly string _serviceName;
        private readonly string _identifier;

        public override string LookupKey => SanitiseLookupKey($"{_serviceName}{_identifier}");

        public NoneDataSourceAttribute(string serviceName, string identifier, SystemType mappingType = default, string description = default)
            : base(mappingType, description)
        {
            _serviceName = serviceName.WhenNotNullOrEmpty(nameof(serviceName));
            _identifier = identifier.WhenNotNullOrEmpty(nameof(identifier));
        }
    }
}