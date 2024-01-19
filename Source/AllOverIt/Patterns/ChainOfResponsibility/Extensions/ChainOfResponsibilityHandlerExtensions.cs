using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.ChainOfResponsibility.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IChainOfResponsibilityHandler{TInput, TOutput}"/> types.</summary>
    public static class ChainOfResponsibilityHandlerExtensions
    {
        private sealed class ChainOfResponsibilityNode<TInput, TOutput> : IChainOfResponsibilityHandler<TInput, TOutput>
        {
            private readonly IChainOfResponsibilityHandler<TInput, TOutput> _first;
            private IChainOfResponsibilityHandler<TInput, TOutput> _last;

            public ChainOfResponsibilityNode(IChainOfResponsibilityHandler<TInput, TOutput> first, IChainOfResponsibilityHandler<TInput, TOutput> next)
            {
                _first = first.WhenNotNull(nameof(first));

                ((IChainOfResponsibilityHandler<TInput, TOutput>) this).SetNext(next);
            }

            public TOutput Handle(TInput state)
            {
                return _first.Handle(state);
            }

            IChainOfResponsibilityHandler<TInput, TOutput> IChainOfResponsibilityHandler<TInput, TOutput>.SetNext(IChainOfResponsibilityHandler<TInput, TOutput> handler)
            {
                // Need to cater for two forms of usage:
                //   var composed = ChainOfResponsibilityHandlerExtensions.Then(handler1, handler2).Then(handler3);
                // and
                //   var composed = handler1.Then(handler2).Then(handler3);
                _last = _last is null
                    ? _first.SetNext(handler)
                    : _last.SetNext(handler);

                return _last;
            }
        }

        private sealed class ChainOfResponsibilityAsyncNode<TInput, TOutput> : IChainOfResponsibilityHandlerAsync<TInput, TOutput>
        {
            private readonly IChainOfResponsibilityHandlerAsync<TInput, TOutput> _first;
            private IChainOfResponsibilityHandlerAsync<TInput, TOutput> _last;

            public ChainOfResponsibilityAsyncNode(IChainOfResponsibilityHandlerAsync<TInput, TOutput> first, IChainOfResponsibilityHandlerAsync<TInput, TOutput> next)
            {
                _first = first.WhenNotNull(nameof(first));

                ((IChainOfResponsibilityHandlerAsync<TInput, TOutput>) this).SetNext(next);
            }

            public Task<TOutput> HandleAsync(TInput state)
            {
                return _first.HandleAsync(state);
            }

            IChainOfResponsibilityHandlerAsync<TInput, TOutput> IChainOfResponsibilityHandlerAsync<TInput, TOutput>.SetNext(IChainOfResponsibilityHandlerAsync<TInput, TOutput> handler)
            {
                // Need to cater for two forms of usage:
                //   var composed = ChainOfResponsibilityHandlerExtensions.Then(handler1, handler2).Then(handler3);
                // and
                //   var composed = handler1.Then(handler2).Then(handler3);
                _last = _last is null
                    ? _first.SetNext(handler)
                    : _last.SetNext(handler);

                return _last;
            }
        }

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

        /// <summary>Composes a collection of <see cref="IChainOfResponsibilityHandlerAsync{TInput, TOutput}"/> handlers so they are chained in the provided order.</summary>
        /// <typeparam name="TInput">The type of the input state provided to the handler.</typeparam>
        /// <typeparam name="TOutput">The type of the output state provided to the handler. This can be the same as <typeparamref name="TInput"/>.</typeparam>
        /// <param name="handlers">The collection of handlers to be chained together.</param>
        /// <returns>The first handler in the chain.</returns>
        public static IChainOfResponsibilityHandlerAsync<TInput, TOutput> Compose<TInput, TOutput>(this IEnumerable<IChainOfResponsibilityHandlerAsync<TInput, TOutput>> handlers)
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

        /// <summary>Creates a new handler that composes two chain of responsibility nodes.</summary>
        /// <typeparam name="TInput">The type of the input state provided to the handler.</typeparam>
        /// <typeparam name="TOutput">The type of the output state provided to the handler. This can be the same as <typeparamref name="TInput"/>.</typeparam>
        /// <param name="first">The first node to handle the request. Depending on the implementation of this handler it may pass the output onto
        /// the <paramref name="next"/> handler.</param>
        /// <param name="next">The first node in the chain to handle the request.</param>
        /// <returns>A new handler that composes two chain of responsibility nodes.</returns>
        public static IChainOfResponsibilityHandler<TInput, TOutput> Then<TInput, TOutput>(this IChainOfResponsibilityHandler<TInput, TOutput> first,
           IChainOfResponsibilityHandler<TInput, TOutput> next)
        {
            _ = first.WhenNotNull(nameof(first));
            _ = next.WhenNotNull(nameof(next));

            return new ChainOfResponsibilityNode<TInput, TOutput>(first, next);
        }

        /// <summary>Creates a new handler that composes two chain of responsibility nodes.</summary>
        /// <typeparam name="TInput">The type of the input state provided to the handler.</typeparam>
        /// <typeparam name="TOutput">The type of the output state provided to the handler. This can be the same as <typeparamref name="TInput"/>.</typeparam>
        /// <param name="first">The first node to handle the request. Depending on the implementation of this handler it may pass the output onto
        /// the <paramref name="next"/> handler.</param>
        /// <param name="next">The first node in the chain to handle the request.</param>
        /// <returns>A new handler that composes two chain of responsibility nodes.</returns>
        public static IChainOfResponsibilityHandlerAsync<TInput, TOutput> Then<TInput, TOutput>(this IChainOfResponsibilityHandlerAsync<TInput, TOutput> first,
          IChainOfResponsibilityHandlerAsync<TInput, TOutput> next)
        {
            _ = first.WhenNotNull(nameof(first));
            _ = next.WhenNotNull(nameof(next));

            return new ChainOfResponsibilityAsyncNode<TInput, TOutput>(first, next);
        }
    }
}