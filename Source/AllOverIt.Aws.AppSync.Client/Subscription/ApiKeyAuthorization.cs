namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class ApiKeyAuthorization : AuthorizationBase
    {
        public ApiKeyAuthorization(string apiKey)
        {
            KeyValues.Add("x-api-key", apiKey);
        }
    }
}