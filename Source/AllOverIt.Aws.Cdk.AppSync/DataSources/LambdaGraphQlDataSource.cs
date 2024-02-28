using AllOverIt.Assertion;

namespace AllOverIt.Aws.Cdk.AppSync.DataSources
{
    /// <summary>Provides information required to declare an AppSync lambda DataSource.</summary>
    public class LambdaGraphQlDataSource : GraphQlDataSourceBase
    {
        /// <summary>The name of the lambda function.</summary>
        public string FunctionName { get; }

        /// <inheritdoc />
        public override string DataSourceName => FunctionName;

        /// <summary>Constructor.</summary>
        /// <param name="functionName">The name of the lambda function.</param>
        /// <param name="description">A description for the datasource.</param>
        public LambdaGraphQlDataSource(string functionName, string description = default)
            : base(description)
        {
            FunctionName = functionName.WhenNotNullOrEmpty(nameof(functionName));
        }
    }
}