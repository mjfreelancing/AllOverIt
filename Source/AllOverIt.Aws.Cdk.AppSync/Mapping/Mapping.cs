using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Aws.Cdk.AppSync.Mapping
{
    public static class Mapping
    {
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

        public static MappingNode ConnectionTemplate()
        {
            return null;
        }
    }
}