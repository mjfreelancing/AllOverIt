using AllOverIt.Aws.Cdk.AppSync.Resolvers;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    /// <summary>Provides extension methods for <see cref="ResolverRegistry"/>.</summary>
    public static class ResolverRegistryExtensions
    {
        /// <summary>Registers the resolver runtime of one or more root query nodes.</summary>
        /// <param name="resolverRegistry">The resolver runtimes registry to register the nodes with.</param>
        /// <param name="nodes">The root nodes, along with any containing child nodes, to register.</param>
        public static void RegisterQueryResolvers(this ResolverRegistry resolverRegistry, params ResolverRuntimeNode[] nodes)
        {
            RegisterRootResolvers(resolverRegistry, "Query", nodes);
        }

        /// <summary>Registers the resolver runtime of one or more root mutation nodes.</summary>
        /// <param name="resolverRegistry">The resolver runtimes registry to register the nodes with.</param>
        /// <param name="nodes">The root nodes, along with any containing child nodes, to register.</param>
        public static void RegisterMutationResolvers(this ResolverRegistry resolverRegistry, params ResolverRuntimeNode[] nodes)
        {
            RegisterRootResolvers(resolverRegistry, "Mutation", nodes);
        }

        /// <summary>Registers the resolver runtime of one or more root subscription nodes.</summary>
        /// <param name="resolverRegistry">The resolver runtimes registry to register the nodes with.</param>
        /// <param name="nodes">The root nodes, along with any containing child nodes, to register.</param>
        public static void RegisterSubscriptionResolvers(this ResolverRegistry resolverRegistry, params ResolverRuntimeNode[] nodes)
        {
            RegisterRootResolvers(resolverRegistry, "Subscription", nodes);
        }

        private static void RegisterRootResolvers(this ResolverRegistry resolverRegistry, string parentNode, params ResolverRuntimeNode[] nodes)
        {
            foreach (var node in nodes)
            {
                RegisterNodeResolvers(resolverRegistry, parentNode, node);
            }
        }

        private static void RegisterNodeResolvers(ResolverRegistry resolverRegistry, string parentNode, ResolverRuntimeNode node)
        {
            var nodeName = $"{parentNode}.{node.Name}";

            resolverRegistry.RegisterResolver(nodeName, node.ResolverRuntime);

            foreach (var child in node.Children ?? [])
            {
                RegisterNodeResolvers(resolverRegistry, nodeName, child);
            }
        }
    }
}