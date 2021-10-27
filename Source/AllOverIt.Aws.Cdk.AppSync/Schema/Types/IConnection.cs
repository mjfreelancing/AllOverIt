namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    /// <summary>Represents a Graphql Connection.</summary>
    public interface IConnection
    {
        /// <summary>Provides the total count for the connection.</summary>
        public int TotalCount();

        /// <summary>Provides pagination information for the connection.</summary>
        public IPageInfo PageInfo();
    }

    /// <summary>Represents a Graphql Connection with nodes and edges.</summary>
    /// <typeparam name="TEdge">The edge type.</typeparam>
    /// <typeparam name="TNode">The node type.</typeparam>
    public interface IConnection<out TEdge, out TNode> : INodesConnection<TNode>, IEdgesConnection<TEdge>
    {
    }
}