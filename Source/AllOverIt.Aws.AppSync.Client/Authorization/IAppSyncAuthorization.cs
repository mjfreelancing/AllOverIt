using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Authorization
{
    public interface IAppSyncAuthorization
    {
        IDictionary<string, string> KeyValues { get; }
    }
}