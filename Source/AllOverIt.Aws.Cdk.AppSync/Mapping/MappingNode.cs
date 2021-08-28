using System.Collections.Generic;

namespace AllOverIt.Aws.Cdk.AppSync.Mapping
{
    public sealed class MappingNode
    {
        public string Name { get; set; }
        public string RequestMapping { get; set; }
        public string ResponseMapping { get; set; }
        public IEnumerable<MappingNode> Children { get; set; }
    }
}