namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    public sealed class LambdaDataSourceAttribute : DataSourceAttribute
    {
        public string ServiceName { get; }
        public string FunctionName { get; }
        public override string LookupKey => $"{ServiceName}{FunctionName}";

        public LambdaDataSourceAttribute(string serviceName, string functionName)
        {
            ServiceName = serviceName;
            FunctionName = functionName;
        }
    }
}