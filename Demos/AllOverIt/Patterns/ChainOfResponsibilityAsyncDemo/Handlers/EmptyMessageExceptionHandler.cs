using AllOverIt.Extensions;

namespace ChainOfResponsibilityAsyncDemo.Handlers
{
    public sealed class EmptyMessageExceptionHandler : QueueMessageHandlerBase
    {
        public override async Task<QueueMessageHandlerState?> HandleAsync(QueueMessageHandlerState state, CancellationToken cancellationToken)
        {
            // Known to not be null in this demo
            var payload = state.QueueMessage.Payload!;

            if (payload.IsEmpty())
            {
                Console.WriteLine(" >> Handling an empty message...");

                // do something async in the handler
                await Task.Delay(100, cancellationToken);

                return await AbandonAsync(state, cancellationToken);
            }

            Console.WriteLine("Payload is empty, trying the next handler.");

            // not handled, so move onto the next handler
            return await base.HandleAsync(state, cancellationToken);
        }
    }
}