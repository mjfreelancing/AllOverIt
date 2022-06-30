namespace AllOverIt.Aws.AppSync.Client.Authorization
{
    /// <summary>Initialized with an 'authorization' authorization header key-value pair. This is applicable to
    /// Cognito, OIDC, and lambda authorization.</summary>
    public sealed class AppSyncAuthorization : AppSyncAuthorizationBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="authToken">The authorization token.</param>
        public AppSyncAuthorization(string authToken)
        {
            KeyValues.Add("authorization", authToken);
        }
    }
}