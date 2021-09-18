namespace AllOverIt.Aws.AppSync.Client.Authorization
{
    public sealed class AppSyncCognitoAuthorization : AppSyncAuthorizationBase
    {
        public AppSyncCognitoAuthorization(string accessToken)
        {
            KeyValues.Add("authorization", accessToken);
        }
    }
}