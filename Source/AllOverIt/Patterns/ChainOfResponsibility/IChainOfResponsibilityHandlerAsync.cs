using System.Threading.Tasks;

namespace AllOverIt.Patterns.ChainOfResponsibility
{
    /// <summary>An interface that describes an asynchronous implementation of the Chain Of Responsibility pattern.</summary>
    /// <typeparam name="TInput">The input state type.</typeparam>
    /// <typeparam name="TOutput">The output state type.</typeparam>
    public interface IChainOfResponsibilityHandlerAsync<TInput, TOutput>
    {
        /// <summary>Sets the next handler in the chain.</summary>
        /// <param name="handler">The next handler that may choose to process a request.</param>
        /// <returns>The original handler to allow for a fluent syntax.</returns>
        IChainOfResponsibilityHandlerAsync<TInput, TOutput> SetNext(IChainOfResponsibilityHandlerAsync<TInput, TOutput> handler);

        /// <summary>Potentially handles a given request using the provided state.</summary>
        /// <param name="state">Contains the request and possibly other state information to potentially be processed
        /// by the current handler.</param>
        /// <returns>An output state.</returns>
        Task<TOutput> HandleAsync(TInput state);
    }
}