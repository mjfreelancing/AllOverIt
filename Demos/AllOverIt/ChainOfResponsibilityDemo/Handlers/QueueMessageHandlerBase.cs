using AllOverIt.Patterns.ChainOfResponsibility;

namespace ChainOfResponsibilityDemo.Handlers
{
    public abstract class QueueMessageHandlerBase : ChainOfResponsibilityHandler<QueueMessageHandlerState, QueueMessageHandlerState>
    {
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
    }
}