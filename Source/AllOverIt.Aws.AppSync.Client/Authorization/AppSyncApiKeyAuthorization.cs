namespace AllOverIt.Aws.AppSync.Client.Authorization
{
    public sealed class AppSyncApiKeyAuthorization : AppSyncAuthorizationBase
    {
        public AppSyncApiKeyAuthorization(string apiKey)
        {
            KeyValues.Add("x-api-key", apiKey);
        }
    }
}