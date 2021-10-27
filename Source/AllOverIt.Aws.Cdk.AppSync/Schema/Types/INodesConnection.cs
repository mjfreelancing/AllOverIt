namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    /// <summary>Represents a Graphql Connection with nodes.</summary>
    public interface INodesConnection<out TNode> : IConnection
    {
        /// <summary>The nodes on the connection.</summary>
        public TNode[] Nodes();
    }
}