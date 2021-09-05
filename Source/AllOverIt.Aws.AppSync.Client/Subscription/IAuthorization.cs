using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public interface IAuthorization
    {
        IDictionary<string, string> KeyValues { get; }
    }
}