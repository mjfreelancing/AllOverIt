using System.Runtime.CompilerServices;

namespace AllOverIt.Wpf.Threading
{
    /// <summary>An awaiter that will queue a continuation onto the UI thread. Once awaited
    /// all subsequent code execution will be bound to the UI thread until the context is
    /// changed.</summary>
    public sealed class UIThreadAwaiter : INotifyCompletion
    {
        /// <summary>Always returns false. The <see cref="OnCompleted(Action)"/> method will ensure
        /// the continuation is invoked, or queued to be invoked, on the UI thread.</summary>
        public bool IsCompleted { get; private set; }

        internal UIThreadAwaiter()
        {
        }

        /// <summary>Queues the <paramref name="action"/> on the UI thread.</summary>
        /// <param name="action">The continuation to be queued on the UI thread.</param>
        public void OnCompleted(Action action)
        {
            UIThread.Invoke(() =>
            {
                IsCompleted = true;
                action.Invoke();
            });
        }

        /// <summary>Performs no operation. A required method for the awaiter.</summary>
        public void GetResult()
        {
        }
    }
}