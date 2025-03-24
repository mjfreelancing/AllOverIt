using AllOverIt.Assertion;

namespace AllOverIt.Patterns.ChainOfResponsibility
{
    /// <summary>A base asynchronous 'Chain Of Responsibility' handler.</summary>
    /// <typeparam name="TInput">The input state type.</typeparam>
    /// <typeparam name="TOutput">The output state type.</typeparam>
    public abstract class ChainOfResponsibilityHandlerAsync<TInput, TOutput> : IChainOfResponsibilityHandlerAsync<TInput, TOutput>
    {
        private IChainOfResponsibilityHandlerAsync<TInput, TOutput>? _nextHandler;

        /// <inheritdoc />
        public IChainOfResponsibilityHandlerAsync<TInput, TOutput> SetNext(IChainOfResponsibilityHandlerAsync<TInput, TOutput> handler)
        {
            _nextHandler = handler.WhenNotNull();
            return handler;
        }

        /// <inheritdoc />
        /// <remarks>If the current handler cannot process the request then it should call base.Handle() to give
        /// the next handler in the chain an opportunity to process the request. To terminate the processing
        /// at the current handler do not call the base method.</remarks>
        public virtual Task<TOutput?> HandleAsync(TInput state, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // The last handler passed to SetNext() will have its _nextHandler un-assigned - return default
            // to indicate it's the end of the chain. A better practice would be to include a terminal handler
            // that does not call base.Handle() at the end of its processing.
            if (_nextHandler is null)
            {
                return Task.FromResult<TOutput?>(default);
            }

            return _nextHandler.HandleAsync(state, cancellationToken);
        }
    }
}