using AllOverIt.Aws.Cdk.AppSync.Mapping;
using System.Linq;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    public static class MappingTemplatesExtensions
    {
        public static void RegisterQueryMappings(this MappingTemplates mappingTemplate, params MappingNode[] nodes)
        {
            RegisterRootMappings(mappingTemplate, "Query", nodes);
        }

        public static void RegisterMutationMappings(this MappingTemplates mappingTemplate, params MappingNode[] nodes)
        {
            RegisterRootMappings(mappingTemplate, "Mutation", nodes);
        }

        public static void RegisterSubscriptionMappings(this MappingTemplates mappingTemplate, params MappingNode[] nodes)
        {
            RegisterRootMappings(mappingTemplate, "Subscription", nodes);
        }

        private static void RegisterRootMappings(this MappingTemplates mappingTemplate, string parentNode, params MappingNode[] nodes)
        {
            foreach (var node in nodes)
            {
                RegisterNodeMappings(mappingTemplate, parentNode, node);
            }
        }

        private static void RegisterNodeMappings(MappingTemplates mappingTemplate, string parentNode, MappingNode node)
        {
            var nodeName = $"{parentNode}.{node.Name}";

            mappingTemplate.RegisterMappings(nodeName, node.RequestMapping, node.ResponseMapping);

            foreach (var child in node.Children ?? Enumerable.Empty<MappingNode>())
            {
                RegisterNodeMappings(mappingTemplate, nodeName, child);
            }
        }
    }
}