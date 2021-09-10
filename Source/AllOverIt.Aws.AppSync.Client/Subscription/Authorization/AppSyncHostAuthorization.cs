namespace AllOverIt.Aws.AppSync.Client.Subscription.Authorization
{
    /// <summary>Decorates a <see cref="AppSyncAuthorizationBase"/> with a 'host' header.</summary>
    public sealed class AppSyncHostAuthorization : AppSyncAuthorizationBase
    {
        public AppSyncHostAuthorization(string host, IAppSyncAuthorization authorization)
        {
            KeyValues.Add("host", host);

            foreach (var (key, value) in authorization.KeyValues)
            {
                KeyValues.Add(key, value);
            }
        }
    }
}