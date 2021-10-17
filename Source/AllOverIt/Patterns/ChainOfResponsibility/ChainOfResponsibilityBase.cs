using AllOverIt.Helpers;

namespace AllOverIt.Patterns.ChainOfResponsibility
{
    public abstract class ChainOfResponsibilityBase<TInput, TOutput> : IChainOfResponsibility<TInput, TOutput>
    {
        private IChainOfResponsibility<TInput, TOutput> _nextHandler;

        public IChainOfResponsibility<TInput, TOutput> SetNext(IChainOfResponsibility<TInput, TOutput> handler)
        {
            _nextHandler = handler.WhenNotNull(nameof(handler));
            return handler;
        }

        public virtual TOutput Handle(TInput request)
        {
            // The last handler passed to SetNext() will have its _nextHandler un-assigned - return default
            // to indicate it's the end of the chain. A better practice would be to include a terminal handler
            // that does not call base.Handle() at the end of its processing.
            return _nextHandler == null
              ? default
              : _nextHandler.Handle(request);
        }
    }
}