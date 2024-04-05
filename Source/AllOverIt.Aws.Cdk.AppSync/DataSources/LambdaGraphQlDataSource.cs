using AllOverIt.Assertion;

namespace AllOverIt.Aws.Cdk.AppSync.DataSources
{
    /// <summary>Provides information required to declare an AppSync lambda DataSource.</summary>
    public class LambdaGraphqlDataSource : GraphqlDataSourceBase
    {
        /// <summary>The name of the lambda function.</summary>
        public string FunctionName { get; }

        /// <summary>Constructor.</summary>
        /// <param name="dataSourceName">The DataSource name.</param>
        /// <param name="functionName">The name of the lambda function.</param>
        /// <param name="description">A description for the datasource.</param>
        public LambdaGraphqlDataSource(string dataSourceName, string functionName, string description = default)
            : base(dataSourceName, description)
        {
            FunctionName = functionName.WhenNotNullOrEmpty(nameof(functionName));
        }
    }
}