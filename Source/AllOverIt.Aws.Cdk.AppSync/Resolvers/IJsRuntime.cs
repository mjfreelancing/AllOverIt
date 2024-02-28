using Amazon.CDK.AWS.AppSync;

namespace AllOverIt.Aws.Cdk.AppSync.Resolvers
{
    /// <summary>Represents an AppSync resolver using a code-based request and response mapping.</summary>
    public interface IJsRuntime : IResolverRuntime
    {
        /// <summary>The code for request and response mapping.</summary>
        public string Code { get; }

        /// <summary>The AppSync runtime version.</summary>
        public FunctionRuntime FunctionRuntime { get; }
    }
}