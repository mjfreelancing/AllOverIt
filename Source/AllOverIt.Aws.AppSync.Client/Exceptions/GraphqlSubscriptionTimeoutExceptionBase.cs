using AllOverIt.Helpers;
using System;
using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    public abstract class GraphqlSubscriptionTimeoutExceptionBase : GraphqlTimeoutExceptionBase
    {
        public string Id { get; }

        protected GraphqlSubscriptionTimeoutExceptionBase(string message, string id, TimeSpan timeout)
            : base(message, timeout)
        {
            Id = id;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _ = info.WhenNotNull(nameof(info));

            info.AddValue("Id", Id);

            base.GetObjectData(info, context);
        }

        protected GraphqlSubscriptionTimeoutExceptionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = (string) info.GetValue("Id", typeof(string))!;
        }
    }
}