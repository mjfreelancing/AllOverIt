using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.DataSources
{
    /// <summary>Provides information required to declare an AppSync lambda datasource.</summary>
    public class LambdaDataSourceAttribute : DataSourceAttribute
    {
        /// <summary>The name of the lambda function.</summary>
        public string FunctionName { get; }

        /// <inheritdoc />
        public override string DataSourceName => FunctionName;

        /// <summary>Constructor.</summary>
        /// <param name="functionName">The name of the lambda function.</param>
        /// <param name="mappingType">The mapping type, inheriting <see cref="IRequestResponseMapping"/>, that provides the datasource request and response
        /// mapping. If null, it is expected that the required mapping will be provided via <see cref="MappingTemplates"/>.</param>
        /// <param name="description">A description for the datasource.</param>
        public LambdaDataSourceAttribute(string functionName, SystemType mappingType = default, string description = default)
            : base(mappingType, description)
        {
            FunctionName = functionName.WhenNotNullOrEmpty(nameof(functionName));
        }
    }
}