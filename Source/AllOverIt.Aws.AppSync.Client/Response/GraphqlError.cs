using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Response
{
    public sealed record GraphqlError
    {
        public IEnumerable<GraphqlErrorDetail> Errors { get; init; }
    }
}