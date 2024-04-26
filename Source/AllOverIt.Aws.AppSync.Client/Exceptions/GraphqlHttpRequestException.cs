using AllOverIt.Aws.AppSync.Client.Response;
using AllOverIt.Extensions;
using System.Net;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>This exception is raised when a non-successful graphql query or mutation occurs.</summary>
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
        internal GraphqlHttpRequestException(HttpStatusCode statusCode, IEnumerable<GraphqlErrorDetail> errors, string content)
        {
            StatusCode = statusCode;
            Errors = errors?.AsReadOnlyCollection();    // can be null
            Content = content;
        }
    }
}