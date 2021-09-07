namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    public interface IConnection<out TEdge, out TNode>
    {
        public TEdge[] Edges();
        public TNode[] Nodes();
        public int TotalCount();
        public IPageInfo PageInfo();
    }
}