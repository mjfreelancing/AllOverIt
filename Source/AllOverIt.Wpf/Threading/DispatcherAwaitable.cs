using System.Windows.Threading;

namespace AllOverIt.Wpf.Threading
{
    /// <summary>A <see cref="Dispatcher"/> awaitable providing a <see cref="GetAwaiter"/> that
    /// returns a <see cref="DispatcherAwaiter"/>.</summary>
    public readonly struct DispatcherAwaitable
    {
        private readonly Dispatcher _dispatcher;

        internal DispatcherAwaitable(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <summary>Gets an awaiter for a <see cref="Dispatcher"/> that is of type <see cref="DispatcherAwaiter"/>.</summary>
        /// <returns>An awaiter for a <see cref="Dispatcher"/> that is of type <see cref="DispatcherAwaiter"/>.</returns>
        public DispatcherAwaiter GetAwaiter()
        {
            return new DispatcherAwaiter(_dispatcher);
        }
    }
}