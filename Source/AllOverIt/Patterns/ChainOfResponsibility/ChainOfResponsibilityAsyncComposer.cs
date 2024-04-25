using AllOverIt.Assertion;
using AllOverIt.Patterns.ChainOfResponsibility.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.ChainOfResponsibility
{
    /// <summary>Composes a sequence of 'Chain Of Responsibility' handlers.</summary>
    /// <typeparam name="TInput">The input state type.</typeparam>
    /// <typeparam name="TOutput">The output state type.</typeparam>
    public class ChainOfResponsibilityAsyncComposer<TInput, TOutput>
    {
        private readonly IChainOfResponsibilityHandlerAsync<TInput, TOutput> _firstHandler;

        /// <summary>Constructor.</summary>
        /// <param name="handlers">The collection of handlers to be composed in the order they are provided.</param>
        public ChainOfResponsibilityAsyncComposer(IEnumerable<IChainOfResponsibilityHandlerAsync<TInput, TOutput>> handlers)
        {
            _firstHandler = handlers
                .WhenNotNullOrEmpty(nameof(handlers))
                .Compose();
        }

        /// <summary>Invokes each handler in turn with the provided state until the state is actioned.</summary>
        /// <param name="state">The input state to be processed.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The final output state of the input was processed. If the input state is not processed by one of the handlers
        /// then the default value for <typeparamref name="TInput" /> is returned (null if it is a reference type).</returns>
        public Task<TOutput?> HandleAsync(TInput state, CancellationToken cancellationToken)
        {
            return _firstHandler.HandleAsync(state, cancellationToken);
        }
    }
}