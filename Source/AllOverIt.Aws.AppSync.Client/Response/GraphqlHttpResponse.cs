using System.Net;
using System.Net.Http.Headers;

namespace AllOverIt.Aws.AppSync.Client.Response
{
    public sealed record GraphqlHttpResponse<TResponse> : GraphqlResponseBase<TResponse>
    {
        public HttpStatusCode StatusCode { get; internal set; }
        public HttpResponseHeaders Headers { get; internal set; }
    }
}