using AllOverIt.Aws.Cdk.AppSync.Resolvers;
using Amazon.CDK.AWS.AppSync;

namespace GraphqlSchema.Schema.Resolvers
{
    internal abstract class CodeResolverBase : IJsRuntime
    {
        public string Code { get; protected set; }

        public FunctionRuntime FunctionRuntime { get; protected set; }

        protected CodeResolverBase()
        {
            Code = """
                function request() {
                };

                function response() {
                };
                """;

            FunctionRuntime = FunctionRuntime.JS_1_0_0;
        }
    }
}