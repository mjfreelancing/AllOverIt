using AllOverIt.Patterns.ChainOfResponsibility;
using AllOverIt.Patterns.ChainOfResponsibility.Extensions;

namespace ChainOfResponsibilityDemo.Handlers
{
    internal sealed class QueueBrokerExceptionHandler2
    {
        private readonly IChainOfResponsibilityHandler<QueueMessageHandlerState, QueueMessageHandlerState> _handler
            = new NullMessageExceptionHandler()
                .Then(new EmptyMessageExceptionHandler())
                .Then(new UnhandledExceptionHandler());         // end of the chain

        public QueueMessageHandlerState Handle(QueueMessage queueMessage, QueueBroker queueBroker, Exception exception)
        {
            // Create state that can be passed from one handler to the next
            var state = new QueueMessageHandlerState(queueMessage, queueBroker, exception);

            return _handler.Handle(state);
        }
    }
}