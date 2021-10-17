using System;

namespace ChainOfResponsibilityDemo.Handlers
{
    public sealed class NullMessageExceptionHandler : QueueMessageHandlerBase
    {
        public override QueueMessageHandlerState Handle(QueueMessageHandlerState state)
        {
            var payload = state.QueueMessage.Payload;

            if (payload == null)
            {
                Console.WriteLine("Handling a null message...");
                return Abandon(state);
            }

            // not handled, so move onto the next handler
            return base.Handle(state);
        }
    }
}