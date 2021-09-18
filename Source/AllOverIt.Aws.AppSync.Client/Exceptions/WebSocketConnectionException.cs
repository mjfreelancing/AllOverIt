using AllOverIt.Aws.AppSync.Client.Response;
using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    [Serializable]
    public sealed class WebSocketConnectionException : Exception
    {
        public string ErrorType { get; }
        public IEnumerable<GraphqlErrorDetail> Errors { get; }

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