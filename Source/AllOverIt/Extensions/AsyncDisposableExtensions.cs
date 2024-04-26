using AllOverIt.Assertion;
using AllOverIt.Async;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IAsyncDisposable"/>.</summary>
    public static class AsyncDisposableExtensions
    {
        /// <summary>Registers an <see cref="IAsyncDisposable"/> with a <see cref="CompositeAsyncDisposable"/> so it will be automatically
        /// disposed when the composite it disposed of.</summary>
        /// <typeparam name="TType">The type implementing <see cref="IAsyncDisposable"/>.</typeparam>
        /// <param name="asyncDisposable">The <see cref="IAsyncDisposable"/> instance.</param>
        /// <param name="disposables">The <see cref="CompositeAsyncDisposable"/> taking ownership of <paramref name="asyncDisposable"/>.</param>
        /// <returns>The original <see cref="IAsyncDisposable"/> instance.</returns>
        public static TType DisposeWith<TType>(this TType asyncDisposable, CompositeAsyncDisposable disposables) where TType : IAsyncDisposable
        {
            // Can't test asyncDisposable for null without adding a 'class' constraint
            _ = disposables.WhenNotNull(nameof(disposables));

            disposables.Add(asyncDisposable);

            return asyncDisposable;
        }

        /// <summary>Asynchronously disposes a collection of <see cref="IAsyncDisposable"/> instances.</summary>
        /// <param name="disposables">The collection of disposables to asynchronously dispose of.</param>
        /// <returns>A <see cref="ValueTask"/> that completes when all disposables have been disposed of.</returns>
        public static ValueTask DisposeAllAsync(this IEnumerable<IAsyncDisposable> disposables)
        {
            _ = disposables.WhenNotNull(nameof(disposables));

            var disposableConnections = new CompositeAsyncDisposable(disposables.ToArray());

            return disposableConnections.DisposeAsync();
        }
    }
}