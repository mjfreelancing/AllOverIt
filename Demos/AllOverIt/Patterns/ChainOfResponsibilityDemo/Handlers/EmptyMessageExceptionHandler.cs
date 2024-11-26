using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace ChainOfResponsibilityDemo.Handlers
{
    public sealed class EmptyMessageExceptionHandler : QueueMessageHandlerBase
    {
        public override QueueMessageHandlerState? Handle(QueueMessageHandlerState state)
        {
            var payload = state.QueueMessage.Payload;

            Throw<InvalidOperationException>.WhenNull(payload, "Not expecting the payload to be null.");

            if (payload.IsEmpty())
            {
                Console.WriteLine(" >> Handling an empty message...");
                return Abandon(state);
            }

            Console.WriteLine("Payload is not empty, trying the next handler.");

            // Not handled, so move onto the next handler
            return base.Handle(state);
        }
    }
}