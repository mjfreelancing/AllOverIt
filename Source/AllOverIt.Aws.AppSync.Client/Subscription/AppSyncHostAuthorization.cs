using AllOverIt.Extensions;
using System.Linq;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class AppSyncHostAuthorization : AppSyncAuthorizationBase
    {
        public AppSyncHostAuthorization(string host, IAppSyncAuthorization authorization)
        {
            KeyValues.Add("host", host);

            foreach (var kvp in authorization.KeyValues)
            {
                KeyValues.Add(kvp.Key, kvp.Value);
            }
        }

        // todo: make this an extension method
        public string GetEncodedHeader()
        {
            var headerValues = string.Join(", ", KeyValues.Select(kvp => $@"""{kvp.Key}"": ""{kvp.Value}"""));
            return $@"{{{headerValues}}}".ToBase64();
        }
    }
}