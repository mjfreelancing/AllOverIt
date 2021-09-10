using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Subscription.Authorization
{
    public abstract class AppSyncAuthorizationBase : IAppSyncAuthorization
    {
        public IDictionary<string, string> KeyValues { get; } = new Dictionary<string, string>();
    }
}