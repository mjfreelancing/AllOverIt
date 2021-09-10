namespace AllOverIt.Aws.AppSync.Client.Subscription.Authorization
{
    public sealed class AppSyncApiKeyAuthorization : AppSyncAuthorizationBase
    {
        public AppSyncApiKeyAuthorization(string apiKey)
        {
            KeyValues.Add("x-api-key", apiKey);
        }
    }
}