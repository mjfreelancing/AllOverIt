using AllOverIt.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Patterns.ChainOfResponsibility
{
    public class ChainOfResponsibilityComposer<TInput, TOutput>
    {
        private readonly IChainOfResponsibilityHandler<TInput, TOutput> _firstHandler;

        protected ChainOfResponsibilityComposer(IEnumerable<IChainOfResponsibilityHandler<TInput, TOutput>> handlers)
        {
            _firstHandler = handlers.Compose();
        }

        protected TOutput Handle(TInput state)
        {
            return _firstHandler.Handle(state);
        }
    }
}