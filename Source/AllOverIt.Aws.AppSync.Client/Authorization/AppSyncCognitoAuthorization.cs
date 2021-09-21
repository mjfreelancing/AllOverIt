namespace AllOverIt.Aws.AppSync.Client.Authorization
{
    /// <summary>Contains AppSync Cognito authorization header key-value pairs.</summary>
    public sealed class AppSyncCognitoAuthorization : AppSyncAuthorizationBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="accessToken">The Cognito access token.</param>
        public AppSyncCognitoAuthorization(string accessToken)
        {
            KeyValues.Add("authorization", accessToken);
        }
    }
}