using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Response
{
    public abstract record GraphqlResponseBase<TResponse>
    {
        public TResponse Data { get; init; }

        public IEnumerable<GraphqlErrorDetail> Errors { get; init; }
    }
}