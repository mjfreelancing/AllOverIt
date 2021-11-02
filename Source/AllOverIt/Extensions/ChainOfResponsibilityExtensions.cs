using AllOverIt.Assertion;
using AllOverIt.Patterns.ChainOfResponsibility;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IChainOfResponsibilityHandler{TInput, TOutput}"/> types.</summary>
    public static class ChainOfResponsibilityExtensions
    {
        /// <summary>Composes a collection of <see cref="IChainOfResponsibilityHandler{TInput, TOutput}"/> handlers so they are chained in the provided order.</summary>
        /// <typeparam name="TInput">The type of the input state provided to the handler.</typeparam>
        /// <typeparam name="TOutput">The type of the output state provided to the handler. This can be the same as <typeparamref name="TInput"/>.</typeparam>
        /// <param name="handlers">The collection of handlers to be chained together.</param>
        /// <returns>The first handler in the chain.</returns>
        public static IChainOfResponsibilityHandler<TInput, TOutput> Compose<TInput, TOutput>(this IEnumerable<IChainOfResponsibilityHandler<TInput, TOutput>> handlers)
        {
            var allHandlers = handlers
                .WhenNotNullOrEmpty(nameof(handlers))
                .AsReadOnlyCollection();

            var firstHandler = allHandlers.First();

            // chain all of the handlers together
            _ = allHandlers
                .Aggregate(
                    firstHandler,
                    (current, handler) => current.SetNext(handler)
                );

            return firstHandler;
        }
    }
}