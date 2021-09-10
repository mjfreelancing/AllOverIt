namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public class WebSocketGraphqlResponse<TPayload>
    {
        public string Type { get; set; }
        public TPayload Payload { get; set; }
    }
}