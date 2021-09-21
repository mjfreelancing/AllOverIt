namespace AllOverIt.Aws.AppSync.Client.Authorization
{
    /// <summary>Decorates an <see cref="IAppSyncAuthorization"/> with a 'host' header key-value pair.</summary>
    public sealed class AppSyncHostAuthorization : AppSyncAuthorizationBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="host">The host value.</param>
        /// <param name="authorization">The <see cref="IAppSyncAuthorization"/> being decorated.</param>
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