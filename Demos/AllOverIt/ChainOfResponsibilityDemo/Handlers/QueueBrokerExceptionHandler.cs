using System;
using System.Linq;
using AllOverIt.Patterns.ChainOfResponsibility;

namespace ChainOfResponsibilityDemo.Handlers
{
    public sealed class QueueBrokerExceptionHandler
    {
        private readonly QueueMessageHandlerBase _firstHandler;

        public QueueBrokerExceptionHandler()
        {
            var handlers = new QueueMessageHandlerBase[]
            {
                new NullMessageExceptionHandler(),
                new EmptyMessageExceptionHandler(),
                new UnhandledExceptionHandler()     // end of the chain
            };

            _firstHandler = handlers.First();

            // chain all of the handlers together
            handlers
                .Aggregate<QueueMessageHandlerBase,
                    IChainOfResponsibility<QueueMessageHandlerState, QueueMessageHandlerState>>(
                    _firstHandler,
                    (current, handler) => current.SetNext(handler)
                );
        }

        public QueueMessageHandlerState Handle(QueueMessage queueMessage, QueueBroker queueBroker, Exception exception)
        {
            // Create state that can be passed from one handler to the next
            var state = new QueueMessageHandlerState(queueMessage, queueBroker, exception);

            // Start with the first handler....
            return _firstHandler.Handle(state);
        }
    }
}