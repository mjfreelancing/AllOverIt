using System;
using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>Base class for all subscription related timeout exceptions.</summary>
    public abstract class SubscriptionTimeoutExceptionBase : TimeoutExceptionBase
    {
        public string Id { get; }

        protected SubscriptionTimeoutExceptionBase(string id, TimeSpan timeout)
            : base(timeout)
        {
            Id = id;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);

            base.GetObjectData(info, context);
        }

        protected SubscriptionTimeoutExceptionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = info.GetString("Id");
        }
    }
}