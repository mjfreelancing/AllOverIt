using AllOverIt.Helpers;

namespace AllOverIt.Patterns.ChainOfResponsibility
{
    public abstract class ChainOfResponsibilityHandler<TInput, TOutput> : IChainOfResponsibilityHandler<TInput, TOutput>
    {
        private IChainOfResponsibilityHandler<TInput, TOutput> _nextHandler;

        /// <inheritdoc />
        public IChainOfResponsibilityHandler<TInput, TOutput> SetNext(IChainOfResponsibilityHandler<TInput, TOutput> handler)
        {
            _nextHandler = handler.WhenNotNull(nameof(handler));
            return handler;
        }

        /// <inheritdoc />
        /// <remarks>If the current handler cannot process the request then it should call base.Handle() to give
        /// the next handler in the chain an opportunity to process the request. To terminate the processing
        /// at the current handler do not call the base method.</remarks>
        public virtual TOutput Handle(TInput state)
        {
            // The last handler passed to SetNext() will have its _nextHandler un-assigned - return default
            // to indicate it's the end of the chain. A better practice would be to include a terminal handler
            // that does not call base.Handle() at the end of its processing.
            return _nextHandler == null
              ? default
              : _nextHandler.Handle(state);
        }
    }
}