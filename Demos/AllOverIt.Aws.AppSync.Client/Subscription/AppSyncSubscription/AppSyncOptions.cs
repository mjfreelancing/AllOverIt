namespace AppSyncSubscription
{
    public sealed class AppSyncOptions
    {
        // AppSync's graphql host Url without https:// or /graphql suffix, such as example123.appsync-api.ap-southeast-2.amazonaws.com
        public string Host { get; set; }

        // If provided, the full custom real-time Url, but without https://
        public string RealtimeUrl { get; set; }

        // graphql Api Key
        public string ApiKey { get; set; }
    }
}