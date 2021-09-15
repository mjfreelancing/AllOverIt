namespace AppSyncSubscription
{
    public sealed class AppSyncOptions
    {
        public string ApiHost { get; set; }         // graphql host (no https:// or /graphql suffix, such as example123.appsync-api.ap-southeast-2.amazonaws.com)
        public string RealTimeHost { get; set; }
        public string ApiKey { get; set; }          // graphql Api Key
    }
}