namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class AppSyncApiKeyAuthorization : AppSyncAuthorizationBase
    {
        public AppSyncApiKeyAuthorization(string apiKey)
        {
            KeyValues.Add("x-api-key", apiKey);
        }
    }
}