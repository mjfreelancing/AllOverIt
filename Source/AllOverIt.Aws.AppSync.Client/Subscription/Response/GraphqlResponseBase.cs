using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Subscription.Response
{
    public abstract class GraphqlResponseBase<TResponse>
    {
        public TResponse Data { get; init; }

        public IEnumerable<GraphqlErrorDetail> Errors { get; init; }
    }
}