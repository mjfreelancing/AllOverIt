using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Aws.Cdk.AppSync.Mapping
{
    /// <summary>Contains mapping helper functions.</summary>
    public static class Mapping
    {
        /// <summary>A factory method that creates a <see cref="MappingNode"/>.</summary>
        /// <param name="name">The dot-notation based name of the node within the GraphQL hierarchy.</param>
        /// <param name="requestMapping">The datasource request mapping for the current node.</param>
        /// <param name="responseMapping">The datasource response mapping for the current node.</param>
        /// <param name="children">Child nodes, if any, of the current node.</param>
        /// <returns>A new <see cref="MappingNode"/> containing the provided request and response mapping details.</returns>
        public static MappingNode Template(string name, string requestMapping, string responseMapping, IEnumerable<MappingNode> children = default)
        {
            return new MappingNode
            {
                Name = name,
                RequestMapping = requestMapping,
                ResponseMapping = responseMapping,
                Children = children ?? Enumerable.Empty<MappingNode>()
            };
        }
    }
}