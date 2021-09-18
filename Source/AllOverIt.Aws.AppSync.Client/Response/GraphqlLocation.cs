namespace AllOverIt.Aws.AppSync.Client.Response
{
    public sealed record GraphqlLocation
    {
        public int Line { get; init; }
        public int Column { get; init; }
    }
}