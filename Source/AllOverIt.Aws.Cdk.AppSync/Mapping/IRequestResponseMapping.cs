namespace AllOverIt.Aws.Cdk.AppSync.Mapping
{
    public interface IRequestResponseMapping
    {
        public string RequestMapping { get; }
        public string ResponseMapping { get; }
    }
}