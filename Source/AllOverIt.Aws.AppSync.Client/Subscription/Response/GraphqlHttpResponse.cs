﻿using System.Net;
using System.Net.Http.Headers;

namespace AllOverIt.Aws.AppSync.Client.Subscription.Response
{
    public sealed class GraphqlHttpResponse<TResponse> : GraphqlResponseBase<TResponse>
    {
        public HttpStatusCode StatusCode { get; internal set; }
        public HttpResponseHeaders Headers { get; internal set; }

    }
}