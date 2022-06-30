namespace AllOverIt.Aws.AppSync.Client.Authorization
{
    /// <summary>Initialized with an 'x-api-key' authorization header key-value pair.</summary>
    public sealed class AppSyncApiKeyAuthorization : AppSyncAuthorizationBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="apiKey">The authorization API Key.</param>
        public AppSyncApiKeyAuthorization(string apiKey)
        {
            KeyValues.Add("x-api-key", apiKey);
        }
    }
}