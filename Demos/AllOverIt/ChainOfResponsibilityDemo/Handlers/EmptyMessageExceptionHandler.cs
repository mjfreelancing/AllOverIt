using AllOverIt.Extensions;
using System;

namespace ChainOfResponsibilityDemo.Handlers
{
    public sealed class EmptyMessageExceptionHandler : QueueMessageHandlerBase
    {
        public override QueueMessageHandlerState Handle(QueueMessageHandlerState state)
        {
            var payload = state.QueueMessage.Payload;

            if (payload != null && payload.IsNullOrEmpty())
            {
                Console.WriteLine("Handling an empty message...");
                return Abandon(state);
            }

            // not handled, so move onto the next handler
            return base.Handle(state);
        }
    }
}