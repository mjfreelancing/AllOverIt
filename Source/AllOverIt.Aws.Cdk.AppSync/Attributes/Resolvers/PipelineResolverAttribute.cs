using AllOverIt.Assertion;
using System;
using System.Linq;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers
{
    public sealed class PipelineResolverAttribute : GraphQlResolverAttribute
    {
        public string[] FunctionNames { get; }

        public PipelineResolverAttribute(Type resolverType, string[] functionNames)
            : base(resolverType)
        {
            FunctionNames = functionNames.WhenNotNullOrEmpty().ToArray();
        }
    }
}