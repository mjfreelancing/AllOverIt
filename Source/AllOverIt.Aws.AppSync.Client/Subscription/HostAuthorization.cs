using AllOverIt.Extensions;
using System.Linq;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class HostAuthorization : AuthorizationBase
    {
        public HostAuthorization(string host, IAuthorization authorization)
        {
            KeyValues.Add("host", host);

            foreach (var kvp in authorization.KeyValues)
            {
                KeyValues.Add(kvp.Key, kvp.Value);
            }
        }

        public string GetEncodedHeader()
        {
            var headerValues = string.Join(", ", KeyValues.Select(kvp => $@"""{kvp.Key}"": ""{kvp.Value}"""));
            return $@"{{{headerValues}}}".ToBase64();
        }
    }
}