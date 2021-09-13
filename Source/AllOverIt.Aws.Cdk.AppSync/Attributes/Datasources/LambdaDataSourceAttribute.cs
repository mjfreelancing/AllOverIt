using AllOverIt.Helpers;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Datasources
{
    public class LambdaDataSourceAttribute : DataSourceAttribute
    {
        public string FunctionName { get; }
        public override string DataSourceName => FunctionName;

        public LambdaDataSourceAttribute(string functionName, SystemType mappingType = default, string description = default)
            : base(mappingType, description)
        {
            FunctionName = functionName.WhenNotNullOrEmpty(nameof(functionName));
        }
    }
}