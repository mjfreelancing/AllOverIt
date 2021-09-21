namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    public interface IEdgesConnection<out TEdge> : IConnection
    {
        public TEdge[] Edges();
    }
}