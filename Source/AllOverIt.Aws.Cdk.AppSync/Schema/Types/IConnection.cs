using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    public interface IConnection<out TEdge, out TNode>
    {
        [SchemaArrayRequired]
        [SchemaTypeRequired]
        public TEdge[] Edges();

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        public TNode[] Nodes();

        [SchemaTypeRequired]
        public int TotalCount();

        // not required since TotalCount can be zero
        public IPageInfo PageInfo();
    }
}