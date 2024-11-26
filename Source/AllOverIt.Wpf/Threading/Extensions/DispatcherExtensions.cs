using AllOverIt.Assertion;
using System.Windows.Threading;

namespace AllOverIt.Wpf.Threading.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="Dispatcher"/>.</summary>
    public static class DispatcherExtensions
    {
        /// <summary>Binds to the provided dispatcher, effectively switching to the thread it is associated with.
        /// As an example, calling <code>Application.Current.Dispatcher.BindTo()</code> will switch from a
        /// worker thread back to the UI thread.</summary>
        /// <param name="dispatcher">The dispatcher to bind to.</param>
        /// <returns>An awaitable object that when awaited completes when the dispatcher is bound to.</returns>
        public static DispatcherAwaitable BindTo(this Dispatcher dispatcher)
        {
            _ = dispatcher.WhenNotNull();

            return new DispatcherAwaitable(dispatcher);
        }
    }
}