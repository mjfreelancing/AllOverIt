using AllOverIt.Aws.Cdk.AppSync.Mapping;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    /// <summary>Provides extension methods for <see cref="MappingTemplates"/>.</summary>
    public static class MappingTemplatesExtensions
    {
        /// <summary>Registers the request and response mappings (via a <see cref="MappingNode"/>) of one or more root query nodes.</summary>
        /// <param name="mappingTemplates">The mapping templates registry to register the nodes with.</param>
        /// <param name="nodes">The root nodes, along with any containing child nodes, to register.</param>
        public static void RegisterQueryMappings(this MappingTemplates mappingTemplates, params MappingNode[] nodes)
        {
            RegisterRootMappings(mappingTemplates, "Query", nodes);
        }

        /// <summary>Registers the request and response mappings (via a <see cref="MappingNode"/>) of one or more root mutation nodes.</summary>
        /// <param name="mappingTemplates">The mapping templates registry to register the nodes with.</param>
        /// <param name="nodes">The root nodes, along with any containing child nodes, to register.</param>
        public static void RegisterMutationMappings(this MappingTemplates mappingTemplates, params MappingNode[] nodes)
        {
            RegisterRootMappings(mappingTemplates, "Mutation", nodes);
        }

        /// <summary>Registers the request and response mappings (via a <see cref="MappingNode"/>) of one or more root subscription nodes.</summary>
        /// <param name="mappingTemplates">The mapping templates registry to register the nodes with.</param>
        /// <param name="nodes">The root nodes, along with any containing child nodes, to register.</param>
        public static void RegisterSubscriptionMappings(this MappingTemplates mappingTemplates, params MappingNode[] nodes)
        {
            RegisterRootMappings(mappingTemplates, "Subscription", nodes);
        }

        private static void RegisterRootMappings(this MappingTemplates mappingTemplates, string parentNode, params MappingNode[] nodes)
        {
            foreach (var node in nodes)
            {
                RegisterNodeMappings(mappingTemplates, parentNode, node);
            }
        }

        private static void RegisterNodeMappings(MappingTemplates mappingTemplates, string parentNode, MappingNode node)
        {
            var nodeName = $"{parentNode}.{node.Name}";

            mappingTemplates.RegisterMappings(nodeName, node.RequestMapping, node.ResponseMapping);

            foreach (var child in node.Children ?? [])
            {
                RegisterNodeMappings(mappingTemplates, nodeName, child);
            }
        }
    }
}