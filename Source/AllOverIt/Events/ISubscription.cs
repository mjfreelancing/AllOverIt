using System;

namespace AllOverIt.Events
{
    public interface ISubscription
    {
        Action<TMessage> GetHandler<TMessage>();
        void Handle<TMessage>(TMessage message);
    }
}
