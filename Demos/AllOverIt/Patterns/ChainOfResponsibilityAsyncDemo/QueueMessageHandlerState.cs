namespace ChainOfResponsibilityAsyncDemo
{
    public sealed class QueueMessageHandlerState
    {
        public QueueMessage QueueMessage { get; }
        public QueueBroker QueueBroker { get; }
        public Exception Exception { get; }

        public QueueMessageHandlerState(QueueMessage queueMessage, QueueBroker queueBroker, Exception exception)
        {
            QueueMessage = queueMessage;
            QueueBroker = queueBroker;
            Exception = exception;
        }
    }
}