namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    /// <summary>Represents a Graphql Connection with edges.</summary>
    public interface IEdgesConnection<out TEdge> : IConnection
    {
        /// <summary>The edges on the connection.</summary>
        public TEdge[] Edges();
    }
}