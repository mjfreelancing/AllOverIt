namespace AllOverIt.Aws.Cdk.AppSync.Resolvers
{
    /// <summary>A node within a GraphQL hierarchy that contains a resolver runtime.</summary>
    public sealed class ResolverRuntimeNode
    {
        /// <summary>The dot-notation based name of the node within the GraphQL hierarchy.</summary>
        /// <remarks>Query root nodes are prefixed with 'Query'. Mutation root nodes are prefixed with 'Mutation'.
        /// Subscription root nodes are prefixed with 'Subscription'.</remarks>
        public string Name { get; set; }

        /// <summary>The resolver runtime containing the request and response handler. This can be an instance
        /// of <see cref="IJsRuntime"/> or <see cref="IVtlRuntime"/>.</summary>
        public IResolverRuntime ResolverRuntime { get; set; }

        /// <summary>Child nodes, if any, of the current node.</summary>
        public IEnumerable<ResolverRuntimeNode> Children { get; set; }
    }
}