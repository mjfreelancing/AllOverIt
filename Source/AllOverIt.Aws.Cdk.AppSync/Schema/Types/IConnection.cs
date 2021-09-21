namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    public interface IConnection
    {
        public int TotalCount();
        public IPageInfo PageInfo();
    }

    public interface IConnection<out TEdge, out TNode> : INodesConnection<TNode>, IEdgesConnection<TEdge>
    {
    }
}