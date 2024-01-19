using AllOverIt.Assertion;
using System.Threading;

namespace AllOverIt.Wpf.Threading.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="SynchronizationContext"/>.</summary>
    public static class SynchronizationContextExtensions
    {
        /// <summary>Gets an awaiter for a <see cref="SynchronizationContext"/> that returns
        /// a <see cref="SynchronizationContextAwaiter"/>.</summary>
        /// <param name="synchronizationContext">The <see cref="SynchronizationContext"/> instance to await.</param>
        /// <returns>An awaiter for a <see cref="SynchronizationContext"/> that returns a <see cref="SynchronizationContextAwaiter"/>.</returns>
        public static SynchronizationContextAwaiter GetAwaiter(this SynchronizationContext synchronizationContext)
        {
            _ = synchronizationContext.WhenNotNull(nameof(synchronizationContext));

            return new SynchronizationContextAwaiter(synchronizationContext);
        }
    }
}