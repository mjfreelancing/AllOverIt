using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public abstract class AuthorizationBase : IAuthorization
    {
        public IDictionary<string, string> KeyValues { get; } = new Dictionary<string, string>();
    }
}