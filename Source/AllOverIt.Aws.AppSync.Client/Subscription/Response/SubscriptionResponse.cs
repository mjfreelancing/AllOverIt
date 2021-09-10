using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public sealed class SubscriptionResponse<TResponse>
    {
        public TResponse Data { get; set; }

        public IEnumerable<GraphqlErrorDetail> Errors { get; set; }
    }
}