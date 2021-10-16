using AllOverIt.Helpers;

namespace AllOverIt.Patterns.ChainOfResponsibility
{
    public abstract class ChainOfResponsibilityBase<TInput, TOutput> : IChainOfResponsibility<TInput, TOutput>
    {
        private IChainOfResponsibility<TInput, TOutput> _nextHandler;

        public IChainOfResponsibility<TInput, TOutput> SetNext(IChainOfResponsibility<TInput, TOutput> handler)
        {
            _nextHandler = handler.WhenNotNull(nameof(handler));

            // provides a fluent syntax
            return handler;
        }

        public virtual TOutput Handle(TInput request)
        {
            return _nextHandler == null
              ? default
              : _nextHandler.Handle(request);
        }
    }
}