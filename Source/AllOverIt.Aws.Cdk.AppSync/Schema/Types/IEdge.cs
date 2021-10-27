namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    /// <summary>Represents an edge on a connection.</summary>
    /// <typeparam name="TNode">The node type on the edge.</typeparam>
    public interface IEdge<out TNode>
    {
        /// <summary>The edge's node type.</summary>
        public TNode Node();

        /// <summary>A cursor representing the current edge.</summary>
        public string Cursor();
    }
}