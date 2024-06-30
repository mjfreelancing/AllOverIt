using System.Net;
using System.Net.Http.Headers;

namespace AllOverIt.Aws.AppSync.Client.Response
{
    /// <summary>Contains response information for a HTTP-based graphql request (such as a query or mutation).</summary>
    /// <typeparam name="TResponse">The type populated with the required response.</typeparam>
    public sealed class GraphqlHttpResponse<TResponse> : GraphqlResponseBase<TResponse>
    {
        /// <summary>Contains the HTTP Status Code of the response.</summary>
        public HttpStatusCode StatusCode { get; internal set; }

        /// <summary>Contains the HTTP headers on the response.</summary>
        public HttpResponseHeaders? Headers { get; internal set; }      // In practice this will never be null
    }
}