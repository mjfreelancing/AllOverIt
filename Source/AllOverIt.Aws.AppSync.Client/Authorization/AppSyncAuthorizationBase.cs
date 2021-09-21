using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Authorization
{
    /// <summary>Base class for AppSync authorization headers.</summary>
    public abstract class AppSyncAuthorizationBase : IAppSyncAuthorization
    {
        /// <summary>Contains authorization header key-value pairs.</summary>
        public IDictionary<string, string> KeyValues { get; } = new Dictionary<string, string>();
    }
}