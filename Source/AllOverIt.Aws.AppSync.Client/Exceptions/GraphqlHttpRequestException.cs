using AllOverIt.Aws.AppSync.Client.Response;
using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    [Serializable]
    public sealed class GraphqlHttpRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public IEnumerable<GraphqlErrorDetail> Errors { get; }
        public string Content { get; }

        public GraphqlHttpRequestException(HttpStatusCode statusCode, IEnumerable<GraphqlErrorDetail> errors, string content)
        {
            StatusCode = statusCode;
            Errors = errors?.AsReadOnlyCollection();    // can be null
            Content = content;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("StatusCode", StatusCode);
            info.AddValue("Errors", Errors, typeof(IEnumerable<GraphqlErrorDetail>));
            info.AddValue("Content", Content);

            base.GetObjectData(info, context);
        }

        private GraphqlHttpRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            StatusCode = (HttpStatusCode) info.GetValue("StatusCode", typeof(HttpStatusCode))!;
            Errors = (IEnumerable<GraphqlErrorDetail>) info.GetValue("Errors", typeof(IEnumerable<GraphqlErrorDetail>))!;
            Content = info.GetString("Content");
        }
    }
}