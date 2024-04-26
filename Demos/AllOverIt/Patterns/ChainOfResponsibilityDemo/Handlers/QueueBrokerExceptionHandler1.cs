using AllOverIt.Patterns.ChainOfResponsibility;

namespace ChainOfResponsibilityDemo.Handlers
{
    internal sealed class QueueBrokerExceptionHandler1 : ChainOfResponsibilityComposer<QueueMessageHandlerState, QueueMessageHandlerState>
    {
        private static readonly IEnumerable<QueueMessageHandlerBase> Handlers =
        [
            new NullMessageExceptionHandler(),
            new EmptyMessageExceptionHandler(),
            new UnhandledExceptionHandler()         // end of the chain
        ];

        public QueueBrokerExceptionHandler1()
            : base(Handlers)
        {
        }

        public QueueMessageHandlerState Handle(QueueMessage queueMessage, QueueBroker queueBroker, Exception exception)
        {
            // Create state that can be passed from one handler to the next
            var state = new QueueMessageHandlerState(queueMessage, queueBroker, exception);

            // Starts with the first handler...
            return Handle(state);
        }
    }
}