using AllOverIt.Aws.AppSync.Client.Response;
using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>This exception is raised when a non-successful graphql query or mutation occurs.</summary>
    [Serializable]
    public sealed class GraphqlHttpRequestException : Exception
    {
        /// <summary>The HTTP status code.</summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>When not null, this provides all graphql errors associated with the query or mutation request.</summary>
        /// <remarks>The <see cref="StatusCode"/> may be 200 in some circumstances, such as when an invalid query is provided.</remarks>
        public IEnumerable<GraphqlErrorDetail> Errors { get; }

        /// <summary>The raw HTT response content returned.</summary>
        public string Content { get; }

        /// <summary>Constructor.</summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="errors">When not null, this provides all graphql errors associated with the query or mutation request.</param>
        /// <param name="content">The raw HTT response content returned.</param>
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