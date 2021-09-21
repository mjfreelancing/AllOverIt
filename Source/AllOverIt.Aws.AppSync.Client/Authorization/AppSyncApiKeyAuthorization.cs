namespace AllOverIt.Aws.AppSync.Client.Authorization
{
    /// <summary>Contains AppSync ApiKey authorization header key-value pairs.</summary>
    public sealed class AppSyncApiKeyAuthorization : AppSyncAuthorizationBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="apiKey">The authorization ApiKey.</param>
        public AppSyncApiKeyAuthorization(string apiKey)
        {
            KeyValues.Add("x-api-key", apiKey);
        }
    }
}