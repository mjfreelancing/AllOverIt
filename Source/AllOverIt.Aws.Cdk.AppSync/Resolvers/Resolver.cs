using System.Collections.Generic;

namespace AllOverIt.Aws.Cdk.AppSync.Resolvers
{
    /// <summary>Contains resolver helper functions.</summary>
    public static class Resolver
    {
        /// <summary>A factory method that creates a <see cref="ResolverRuntimeNode"/>.</summary>
        /// <param name="name">The dot-notation based name of the node within the GraphQL hierarchy.</param>
        /// <param name="resolverRuntime">The resolver runtime for the current node.</param>
        /// <param name="children">Child nodes, if any, of the current node.</param>
        /// <returns>A new <see cref="ResolverRuntimeNode"/> containing the provided resolver request and response handler details.</returns>
        public static ResolverRuntimeNode Template(string name, IResolverRuntime resolverRuntime, IEnumerable<ResolverRuntimeNode> children = default)
        {
            return new ResolverRuntimeNode
            {
                Name = name,
                ResolverRuntime = resolverRuntime,
                Children = children ?? []
            };
        }
    }
}