using AllOverIt.Helpers;
using System;
using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    public abstract class TimeoutExceptionBase : Exception
    {
        public TimeSpan Timeout { get; }

        protected TimeoutExceptionBase(TimeSpan timeout)
        {
            Timeout = timeout;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _ = info.WhenNotNull(nameof(info));

            info.AddValue("Timeout", Timeout);

            base.GetObjectData(info, context);
        }

        protected TimeoutExceptionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Timeout = (TimeSpan) info.GetValue("Timeout", typeof(TimeSpan))!;
        }
    }
}