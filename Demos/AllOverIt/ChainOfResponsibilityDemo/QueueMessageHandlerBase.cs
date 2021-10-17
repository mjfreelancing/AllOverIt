using AllOverIt.Patterns.ChainOfResponsibility;

namespace ChainOfResponsibilityDemo
{
    public abstract class QueueMessageHandlerBase : ChainOfResponsibilityBase<QueueMessageHandlerState, QueueMessageHandlerState>
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