using AllOverIt.Patterns.ChainOfResponsibility;

namespace ChainOfResponsibilityAsyncDemo.Handlers
{
    public sealed class QueueBrokerExceptionHandler : ChainOfResponsibilityAsyncComposer<QueueMessageHandlerState, QueueMessageHandlerState>
    {
        private static readonly IEnumerable<QueueMessageHandlerBase> _handlers =
        [
            new NullMessageExceptionHandler(),
            new EmptyMessageExceptionHandler(),
            new UnhandledExceptionHandler()         // end of the chain
        ];

        public QueueBrokerExceptionHandler()
            : base(_handlers)
        {
        }

        public Task<QueueMessageHandlerState?> HandleAsync(QueueMessage queueMessage, QueueBroker queueBroker, Exception exception, CancellationToken cancellationToken)
        {
            // Create state that can be passed from one handler to the next
            var state = new QueueMessageHandlerState(queueMessage, queueBroker, exception);

            // Starts with the first handler...
            return HandleAsync(state, cancellationToken);
        }
    }
}