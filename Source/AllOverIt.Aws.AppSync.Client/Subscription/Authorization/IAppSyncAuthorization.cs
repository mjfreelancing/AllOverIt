using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Subscription.Authorization
{
    public interface IAppSyncAuthorization
    {
        IDictionary<string, string> KeyValues { get; }
    }
}