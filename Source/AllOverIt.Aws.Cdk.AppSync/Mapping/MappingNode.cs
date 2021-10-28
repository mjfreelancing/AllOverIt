using System.Collections.Generic;

namespace AllOverIt.Aws.Cdk.AppSync.Mapping
{
    /// <summary>A node within a GraphQL hierarchy that contains a datasource request and response mapping.</summary>
    public sealed class MappingNode
    {
        /// <summary>The dot-notation based name of the node within the GraphQL hierarchy.</summary>
        /// <remarks>Query root nodes are prefixed with 'Query'. Mutation root nodes are prefixed with 'Mutation'.
        /// Subscription root nodes are prefixed with 'Subscription'.</remarks>
        public string Name { get; set; }

        /// <summary>The datasource request mapping for the current node.</summary>
        public string RequestMapping { get; set; }

        /// <summary>The datasource response mapping for the current node.</summary>
        public string ResponseMapping { get; set; }

        /// <summary>Child nodes, if any, of the current node.</summary>
        public IEnumerable<MappingNode> Children { get; set; }
    }
}