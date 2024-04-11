using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChainOfResponsibilityAsyncDemo.Handlers
{
    public sealed class UnhandledExceptionHandler : QueueMessageHandlerBase
    {
        public override Task<QueueMessageHandlerState> HandleAsync(QueueMessageHandlerState state, CancellationToken cancellationToken)
        {
            Console.WriteLine(" >> Handling an unhandled exception... (no more handlers)");

            // This is the end of the chain - just return the final state
            return DeadletterAsync(state, cancellationToken);
        }
    }
}