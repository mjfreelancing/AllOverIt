using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    public interface IConnection<out TEdge, out TNode>
    {
        [SchemaArrayRequired]
        [SchemaTypeRequired]
        public TEdge[] Edges { get; }

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        public TNode[] Nodes { get; }

        [SchemaTypeRequired]
        public int TotalCount { get; }

        // not required since TotalCount can be zero
        public IPageInfo PageInfo { get; }
    }
}