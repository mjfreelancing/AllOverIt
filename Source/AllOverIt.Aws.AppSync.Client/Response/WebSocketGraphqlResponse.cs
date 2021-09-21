namespace AllOverIt.Aws.AppSync.Client.Response
{
    public class WebSocketGraphqlResponse<TPayload>
    {
        public string Type { get; init; }
        public TPayload Payload { get; init; }
    }
}