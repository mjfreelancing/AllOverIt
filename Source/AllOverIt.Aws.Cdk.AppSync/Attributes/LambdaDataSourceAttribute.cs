using AllOverIt.Helpers;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    public class LambdaDataSourceAttribute : DataSourceAttribute
    {
        public string ServiceName { get; }
        public string FunctionName { get; }
        public override string LookupKey => SanitiseLookupKey($"{ServiceName}{FunctionName}");

        public LambdaDataSourceAttribute(string serviceName, string functionName, SystemType mappingType = default, string description = default)
            : base(mappingType, description)
        {
            ServiceName = serviceName.WhenNotNullOrEmpty(nameof(serviceName));
            FunctionName = functionName.WhenNotNullOrEmpty(nameof(functionName));
        }
    }
}