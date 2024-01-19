using AllOverIt.Assertion;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AllOverIt.Wpf.Threading
{
    /// <summary>An awaiter that will queue a continuation on the thread associated with
    /// a <see cref="SynchronizationContext"/>. Once awaited all subsequent code execution
    /// will be bound to this thread until the context is changed.</summary>
    public readonly struct SynchronizationContextAwaiter : INotifyCompletion
    {
        // The 'state' is the continuation to be invoked
        private static readonly SendOrPostCallback SynchronizationCallback = state => ((Action) state).Invoke();

        private readonly SynchronizationContext _synchronizationContext;

        /// <summary>Indicates if the required SynchronizationContext is the same as the current thread's
        /// SynchronizationContext.</summary>
        public bool IsCompleted => _synchronizationContext == SynchronizationContext.Current;

        internal SynchronizationContextAwaiter(SynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext.WhenNotNull(nameof(synchronizationContext));
        }

        /// <summary>Queues the <paramref name="action"/> on the thread associated with the required
        /// synchronization context.</summary>
        /// <param name="action">The action to be queued on the thread associated with the required SynchronizationContext.</param>
        public void OnCompleted(Action action)
        {
            _synchronizationContext.Post(SynchronizationCallback, action);
        }

        /// <summary>Performs no operation. A required method for the awaiter.</summary>
        public void GetResult()
        {
        }
    }
}