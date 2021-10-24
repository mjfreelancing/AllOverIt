using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System.Collections.Generic;

namespace AllOverIt.Patterns.ChainOfResponsibility
{
    /// <summary>Composes a sequence of 'Chain Of Responsibility' handlers.</summary>
    /// <typeparam name="TInput">The input state type.</typeparam>
    /// <typeparam name="TOutput">The output state type.</typeparam>
    public class ChainOfResponsibilityComposer<TInput, TOutput>
    {
        private readonly IChainOfResponsibilityHandler<TInput, TOutput> _firstHandler;

        /// <summary>Constructor.</summary>
        /// <param name="handlers">The collection of handlers to be composed in the order they are provided.</param>
        public ChainOfResponsibilityComposer(IEnumerable<IChainOfResponsibilityHandler<TInput, TOutput>> handlers)
        {
            _firstHandler = handlers
                .WhenNotNullOrEmpty(nameof(handlers))
                .Compose();
        }

        /// <summary>Invokes each handler in turn with the provided state until the state is actioned.</summary>
        /// <param name="state">The input state to be processed.</param>
        /// <returns>The final output state of the input was processed. If the input state is not processed by one of the handlers
        /// then the default value for <typeparamref name="TInput" /> is returned (null if it is a reference type).</returns>
        public TOutput Handle(TInput state)
        {
            return _firstHandler.Handle(state);
        }
    }
}