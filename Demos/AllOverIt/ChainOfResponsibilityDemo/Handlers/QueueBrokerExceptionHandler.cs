using AllOverIt.Patterns.ChainOfResponsibility;
using System;
using System.Collections.Generic;

namespace ChainOfResponsibilityDemo.Handlers
{
    public sealed class QueueBrokerExceptionHandler : ChainOfResponsibilityComposer<QueueMessageHandlerState, QueueMessageHandlerState>
    {
        private static readonly IEnumerable<QueueMessageHandlerBase> Handlers = new List<QueueMessageHandlerBase>
        {
            new NullMessageExceptionHandler(),
            new EmptyMessageExceptionHandler(),
            new UnhandledExceptionHandler() // end of the chain
        };

        public QueueBrokerExceptionHandler()
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