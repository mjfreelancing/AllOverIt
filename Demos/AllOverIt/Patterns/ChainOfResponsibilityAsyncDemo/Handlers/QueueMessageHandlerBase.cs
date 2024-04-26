﻿using AllOverIt.Patterns.ChainOfResponsibility;

namespace ChainOfResponsibilityAsyncDemo.Handlers
{
    public abstract class QueueMessageHandlerBase : ChainOfResponsibilityHandlerAsync<QueueMessageHandlerState, QueueMessageHandlerState>
    {
        // This is demo code - the members would normally be accessing member data
#pragma warning disable CA1822 // Mark members as static
        protected async Task<QueueMessageHandlerState> AbandonAsync(QueueMessageHandlerState state, CancellationToken cancellationToken)
        {
            // do something async in the handler
            await Task.Delay(100, cancellationToken);

            state.QueueBroker.Abandon();

            return state;
        }

        protected async Task<QueueMessageHandlerState> DeadletterAsync(QueueMessageHandlerState state, CancellationToken cancellationToken)
        {
            // do something async in the handler
            await Task.Delay(100, cancellationToken);

            state.QueueBroker.Deadletter();

            return state;
        }
#pragma warning restore CA1822 // Mark members as static
    }
}