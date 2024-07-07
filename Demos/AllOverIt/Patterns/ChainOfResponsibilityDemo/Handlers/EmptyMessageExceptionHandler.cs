using AllOverIt.Extensions;

namespace ChainOfResponsibilityDemo.Handlers
{
    public sealed class EmptyMessageExceptionHandler : QueueMessageHandlerBase
    {
        public override QueueMessageHandlerState? Handle(QueueMessageHandlerState state)
        {
            var payload = state.QueueMessage.Payload;

            if (payload.IsEmpty())
            {
                Console.WriteLine(" >> Handling an empty message...");
                return Abandon(state);
            }

            Console.WriteLine("Payload is empty, trying the next handler.");

            // not handled, so move onto the next handler
            return base.Handle(state);
        }
    }
}