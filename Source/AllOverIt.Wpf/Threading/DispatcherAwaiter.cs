using AllOverIt.Assertion;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace AllOverIt.Wpf.Threading
{
    /// <summary>An awaiter that will queue a continuation on the thread associated with
    /// a <see cref="Dispatcher"/>. Once awaited all subsequent code execution will be bound
    /// to this thread until the context is changed.</summary>
    public readonly struct DispatcherAwaiter : INotifyCompletion
    {
        private readonly Dispatcher _dispatcher;

        /// <summary>Returns <see langword="true"/> when the calling thread is associated with the
        /// dispatcher passed to the constructor.</summary>
        public bool IsCompleted => _dispatcher.CheckAccess();

        internal DispatcherAwaiter(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher.WhenNotNull(nameof(dispatcher));
        }

        /// <summary>Queues the <paramref name="action"/> on the thread associated with the dispatcher
        /// passed to the constructor.</summary>
        /// <param name="action">The action to be queued on the UI thread.</param>
        public void OnCompleted(Action action)
        {
            _dispatcher.BeginInvoke(action);
        }

        /// <summary>Performs no operation. A required method for the awaiter.</summary>
        public void GetResult()
        {
        }
    }
}