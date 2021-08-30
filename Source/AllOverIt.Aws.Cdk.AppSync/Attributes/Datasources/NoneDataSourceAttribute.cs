using AllOverIt.Helpers;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Datasources
{
    public class NoneDataSourceAttribute : DataSourceAttribute
    {
        private readonly string _identifier;

        public override string LookupKey => SanitiseLookupKey(_identifier);

        public NoneDataSourceAttribute(string identifier, SystemType mappingType = default, string description = default)
            : base(mappingType, description)
        {
            _identifier = identifier.WhenNotNullOrEmpty(nameof(identifier));
        }
    }
}