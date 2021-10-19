using AllOverIt.Helpers;
using AllOverIt.Patterns.ChainOfResponsibility;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Extensions
{
    public static class ChainOfResponsibilityExtensions
    {
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