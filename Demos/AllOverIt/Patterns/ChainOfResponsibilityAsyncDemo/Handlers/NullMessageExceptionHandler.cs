namespace ChainOfResponsibilityAsyncDemo.Handlers
{
    public sealed class NullMessageExceptionHandler : QueueMessageHandlerBase
    {
        public override async Task<QueueMessageHandlerState?> HandleAsync(QueueMessageHandlerState state, CancellationToken cancellationToken)
        {
            var payload = state.QueueMessage.Payload;

            if (payload == null)
            {
                Console.WriteLine(" >> Handling a null message...");

                // do something async in the handler
                await Task.Delay(100, cancellationToken);

                return await AbandonAsync(state, cancellationToken);
            }

            Console.WriteLine("Payload is not null, trying the next handler.");

            // not handled, so move onto the next handler
            return await base.HandleAsync(state, cancellationToken);
        }
    }
}