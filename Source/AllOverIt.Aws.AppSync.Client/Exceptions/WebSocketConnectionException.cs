using AllOverIt.Aws.AppSync.Client.Response;
using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>Contains the reason for a websocket error that occurred during a connection request.</summary>
    [Serializable]
    public sealed class WebSocketConnectionException : Exception
    {
        /// <summary>The graphql error type.</summary>
        public string ErrorType { get; }
        
        /// <summary>A collection of graphql errors as reported by AppSync.</summary>
        public IEnumerable<GraphqlErrorDetail> Errors { get; }

        /// <summary>Constructor.</summary>
        /// <param name="error">The error payload reported by AppSync.</param>
        public WebSocketConnectionException(WebSocketGraphqlResponse<GraphqlError> error)
            : base(error.Type)
        {
            ErrorType = error.Type;
            Errors = error.Payload.Errors.AsReadOnlyCollection();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ErrorType", ErrorType);
            info.AddValue("Errors", Errors, typeof(IEnumerable<GraphqlErrorDetail>));

            base.GetObjectData(info, context);
        }

        private WebSocketConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ErrorType = info.GetString("ErrorType");
            Errors = (IEnumerable<GraphqlErrorDetail>) info.GetValue("Errors", typeof(IEnumerable<GraphqlErrorDetail>));
        }
    }
}