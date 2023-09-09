using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Response
{
    /// <summary>A base class for all graphql responses.</summary>
    /// <typeparam name="TResponse">The response type populated with the expected response data.</typeparam>
    public abstract class GraphqlResponseBase<TResponse>
    {
        /// <summary>In the absence of errors, this contains the response data.</summary>
        public TResponse Data { get; init; }

        /// <summary>When a query error occurs this contains the error information reported by AppSync.</summary>
        public IEnumerable<GraphqlErrorDetail> Errors { get; init; }
    }
}