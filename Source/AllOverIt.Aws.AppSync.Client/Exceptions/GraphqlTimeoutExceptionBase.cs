using AllOverIt.Helpers;
using System;
using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    public abstract class GraphqlTimeoutExceptionBase : Exception
    {
        public TimeSpan Timeout { get; }

        protected GraphqlTimeoutExceptionBase(string message, TimeSpan timeout)
            : base(message)
        {
            Timeout = timeout;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _ = info.WhenNotNull(nameof(info));

            info.AddValue("Timeout", Timeout);

            base.GetObjectData(info, context);
        }

        protected GraphqlTimeoutExceptionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Timeout = (TimeSpan) info.GetValue("Timeout", typeof(TimeSpan))!;
        }
    }
}