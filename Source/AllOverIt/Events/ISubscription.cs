namespace AllOverIt.Events
{
    internal interface ISubscription
    {
        // Gets a subscribed handler for a given message type
        Action<TMessage>? GetHandler<TMessage>();

        // Gets a subscribed handler for a given message type and invokes it with the provided message
        void Handle<TMessage>(TMessage message);
    }
}
