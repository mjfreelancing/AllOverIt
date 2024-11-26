using AllOverIt.Aws.AppSync.Client.Response;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>Contains the reason for a websocket error that occurred during a connection request.</summary>
    public sealed class WebSocketConnectionException : Exception
    {
        /// <summary>The graphql error type.</summary>
        public string ErrorType { get; }

        /// <summary>A collection of graphql errors as reported by AppSync.</summary>
        public GraphqlErrorDetail[] Errors { get; }

        /// <summary>Constructor.</summary>
        /// <param name="error">The error payload reported by AppSync.</param>
        internal WebSocketConnectionException(WebSocketGraphqlResponse<GraphqlError> error)
            : base(error.Type)
        {
            ErrorType = error.Type;
            Errors = error.Payload.Errors;
        }
    }
}