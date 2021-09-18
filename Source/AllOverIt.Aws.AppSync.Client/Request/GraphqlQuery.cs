namespace AllOverIt.Aws.AppSync.Client.Request
{
    public sealed class GraphqlQuery
    {
        public string Query { get; set; }
        public object Variables { get; set; }
    }
}