namespace AllOverIt.Aws.AppSync.Client.Response
{
    public abstract class WebSocketGraphqlResponse<TPayload>
    {
        public string Type { get; init; }
        public TPayload Payload { get; init; }
    }
}