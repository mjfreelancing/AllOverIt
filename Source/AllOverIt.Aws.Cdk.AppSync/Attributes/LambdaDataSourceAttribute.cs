using AllOverIt.Helpers;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    public sealed class LambdaDataSourceAttribute : DataSourceAttribute
    {
        public string ServiceName { get; }
        public string FunctionName { get; }
        public override string LookupKey => SanitiseLookupKey($"{ServiceName}{FunctionName}");

        public LambdaDataSourceAttribute(string serviceName, string functionName, string description = default)
            : base(description)
        {
            ServiceName = serviceName.WhenNotNullOrEmpty(nameof(serviceName));
            FunctionName = functionName.WhenNotNullOrEmpty(nameof(functionName));
        }
    }
}