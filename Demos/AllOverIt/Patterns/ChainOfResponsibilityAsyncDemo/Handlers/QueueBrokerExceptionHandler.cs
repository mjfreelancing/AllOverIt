using AllOverIt.Patterns.ChainOfResponsibility;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChainOfResponsibilityAsyncDemo.Handlers
{
    public sealed class QueueBrokerExceptionHandler : ChainOfResponsibilityAsyncComposer<QueueMessageHandlerState, QueueMessageHandlerState>
    {
        private static readonly IEnumerable<QueueMessageHandlerBase> Handlers =
        [
            new NullMessageExceptionHandler(),
            new EmptyMessageExceptionHandler(),
            new UnhandledExceptionHandler()         // end of the chain
        ];

        public QueueBrokerExceptionHandler()
            : base(Handlers)
        {
        }

        public Task<QueueMessageHandlerState> HandleAsync(QueueMessage queueMessage, QueueBroker queueBroker, Exception exception, CancellationToken cancellationToken)
        {
            // Create state that can be passed from one handler to the next
            var state = new QueueMessageHandlerState(queueMessage, queueBroker, exception);

            // Starts with the first handler...
            return HandleAsync(state, cancellationToken);
        }
    }
}