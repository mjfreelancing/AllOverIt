using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    public interface IEdge<out TNode>
    {
        [SchemaTypeRequired]
        public TNode Node();

        [SchemaTypeRequired]
        public string Cursor();
    }
}