namespace AllOverIt.Events
{
    internal interface IAsyncSubscription
    {
        // Gets a subscribed handler for a given message type
        Func<TMessage, Task> GetHandler<TMessage>();

        // Gets a subscribed handler for a given message type and asynchronously invokes it with the provided message
        Task HandleAsync<TMessage>(TMessage message);
    }
}
