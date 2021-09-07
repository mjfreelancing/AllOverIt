namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    public interface IEdge<out TNode>
    {
        public TNode Node();
        public string Cursor();
    }
}