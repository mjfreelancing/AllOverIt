namespace ChainOfResponsibilityDemo.Handlers
{
    public sealed class NullMessageExceptionHandler : QueueMessageHandlerBase
    {
        public override QueueMessageHandlerState? Handle(QueueMessageHandlerState state)
        {
            var payload = state.QueueMessage.Payload;

            if (payload is null)
            {
                Console.WriteLine(" >> Handling a null message...");
                return Abandon(state);
            }

            Console.WriteLine("Payload is not null, trying the next handler.");

            // not handled, so move onto the next handler
            return base.Handle(state);
        }
    }
}