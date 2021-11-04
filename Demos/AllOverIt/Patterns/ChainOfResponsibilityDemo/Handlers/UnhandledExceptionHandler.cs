using System;

namespace ChainOfResponsibilityDemo.Handlers
{
    public sealed class UnhandledExceptionHandler : QueueMessageHandlerBase
    {
        public override QueueMessageHandlerState Handle(QueueMessageHandlerState state)
        {
            Console.WriteLine("Handling an unhandled exception...");

            // This is the end of the chain - just return the final state
            return Deadletter(state);
        }
    }
}