using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Subscription.Response
{
    public sealed class GraphqlError
    {
        public IEnumerable<GraphqlErrorDetail> Errors { get; init; }
    }
}