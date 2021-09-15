namespace AppSyncSubscription
{
    public sealed class AppSyncOptions
    {
        // AppSync's graphql host Url without https:// or /graphql suffix, such as example123.appsync-api.ap-southeast-2.amazonaws.com
        public string ApiHost { get; set; }

        // If provided, the full custom realtime Url, but without https://
        public string RealTimeHost { get; set; }

        // graphql Api Key
        public string ApiKey { get; set; }
    }
}