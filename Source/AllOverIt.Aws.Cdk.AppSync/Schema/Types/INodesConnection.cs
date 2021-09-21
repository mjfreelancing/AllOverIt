namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    public interface INodesConnection<out TNode> : IConnection
    {
        public TNode[] Nodes();
    }
}