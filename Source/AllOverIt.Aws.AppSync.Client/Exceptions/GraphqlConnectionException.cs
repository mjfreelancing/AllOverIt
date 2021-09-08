﻿using AllOverIt.Aws.AppSync.Client.Subscription;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    [Serializable]
    public sealed class GraphqlConnectionException : Exception
    {
        public string ErrorType { get; }
        public IEnumerable<GraphqlErrorDetail> Errors { get; }

        public GraphqlConnectionException(WebSocketGraphqlResponse<GraphqlError> error)
            : base(error.Type)
        {
            ErrorType = error.Type;
            Errors = error.Payload.Errors.AsReadOnlyCollection();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _ = info.WhenNotNull(nameof(info));

            info.AddValue("ErrorType", ErrorType);
            info.AddValue("Errors", Errors, typeof(IEnumerable<GraphqlErrorDetail>));

            base.GetObjectData(info, context);
        }

        private GraphqlConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ErrorType = info.GetString("ErrorType");
            Errors = (IEnumerable<GraphqlErrorDetail>) info.GetValue("Errors", typeof(IEnumerable<GraphqlErrorDetail>));
        }
    }
}