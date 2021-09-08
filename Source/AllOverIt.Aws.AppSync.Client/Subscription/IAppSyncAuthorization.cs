using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public interface IAppSyncAuthorization
    {
        IDictionary<string, string> KeyValues { get; }
    }
}