using System;
using System.Runtime.Serialization;
using AllOverIt.Helpers;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    public abstract class GraphqlTimeoutExceptionBase : Exception
    {
        public TimeSpan TimeoutPeriod { get; }

        protected GraphqlTimeoutExceptionBase(string message, TimeSpan timeoutPeriod)
            : base(message)
        {
            TimeoutPeriod = timeoutPeriod;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _ = info.WhenNotNull(nameof(info));

            info.AddValue("TimeoutPeriod", TimeoutPeriod);

            base.GetObjectData(info, context);
        }

        protected GraphqlTimeoutExceptionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            TimeoutPeriod = (TimeSpan) info.GetValue("TimeoutPeriod", typeof(TimeSpan))!;
        }
    }
}