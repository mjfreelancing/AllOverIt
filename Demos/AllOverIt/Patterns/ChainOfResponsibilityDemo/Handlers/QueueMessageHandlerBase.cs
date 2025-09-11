using AllOverIt.Patterns.ChainOfResponsibility;

namespace ChainOfResponsibilityDemo.Handlers
{
    public abstract class QueueMessageHandlerBase : ChainOfResponsibilityHandler<QueueMessageHandlerState, QueueMessageHandlerState>
    {
        // This is demo code - the members would normally be accessing member data
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static
        protected QueueMessageHandlerState Abandon(QueueMessageHandlerState state)
        {
            state.QueueBroker.Abandon();

            return state;
        }

        protected QueueMessageHandlerState Deadletter(QueueMessageHandlerState state)
        {
            state.QueueBroker.Deadletter();

            return state;
        }
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079 // Remove unnecessary suppression
    }
}