using AllOverIt.Assertion;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AllOverIt.Wpf.Threading
{
    /// <summary>A static class providing helper methods to invoke method calls on the UI thread and
    /// support binding (a continuation) to the UI or a worker thread.</summary>
    public static class UIThread
    {
        /// <summary>Indicates if the calling thread is the UI thread.</summary>
        /// <returns><see langword="True"/> if the calling thread is the UI thread, otherwise <see langword="False"/>.</returns>
        public static bool IsBound()
        {
            return GetIsBound(out _);
        }

        /// <summary>Binds to the application's UI thread dispatcher so all future code execution
        /// is performed on the UI thread until the synchronization context is changed.</summary>
        /// <returns>A <see cref="UIThreadAwaitable"/> that when awaited will bind to the UI thread.</returns>
        public static UIThreadAwaitable BindToAsync()
        {
            return new UIThreadAwaitable();
        }

        /// <summary>Invokes the provided <paramref name="action"/> on the UI thread.</summary>
        /// <param name="action">The action to be invoked on the UI thread.</param>
        /// <returns>A <see cref="Task"/> that completes when the action has been invoked on the UI thread.</returns>
        public static async Task InvokeAsync(Func<Task> action)
        {
            _ = action.WhenNotNull(nameof(action));

            // Switch to the UI thread
            await BindToAsync();

            // Execute the action on the UI thread
            await action.Invoke().ConfigureAwait(false);
        }

        /// <summary>Invokes the provided asynchronous <paramref name="action"/> on the UI thread.</summary>
        /// <typeparam name="TResult">The result type returned by the asynchronous action.</typeparam>
        /// <param name="action">The asynchronous action to be invoked on the UI thread.</param>
        /// <returns>A <see cref="Task{TResult}"/> that completes when the action has been invoked on the UI thread.</returns>
        public static async Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> action)
        {
            _ = action.WhenNotNull(nameof(action));

            // Switch to the UI thread
            await BindToAsync();

            // Execute the action on the UI thread
            return await action.Invoke().ConfigureAwait(false);
        }

        /// <summary>Schedules the provided <paramref name="action"/> to be invoked on the UI thread. If the calling thread
        /// is the UI thread then the action will be invoked immediately, otherwise this method will return immediately and
        /// the action will be invoked when it is processed by the UI thread's message queue. An example use
        /// case is when capturing an exception in an <c>async void</c> method that needs to be re-thrown on the UI thread.
        /// <code>
        /// RunSomeTask(cancellationToken)
        ///   .FireAndForget(edi =>
        ///   {
        ///     UIThread.Invoke(() => edi.Throw());
        ///   });
        /// </code>
        /// </summary>
        /// <param name="action">The action to be invoked on the UI thread.</param>
        public static void Invoke(Action action)
        {
            _ = action.WhenNotNull(nameof(action));

            if (GetIsBound(out var dispatcher))
            {
                action.Invoke();
            }
            else
            {
                _ = dispatcher.BeginInvoke(DispatcherPriority.Normal, () => action.Invoke());
            }
        }

        /// <summary>Schedules the provided <paramref name="action"/> to be invoked on a worker thread. If the calling thread
        /// is not the UI thread then the action will be invoked immediately, otherwise this method will return immediately and
        /// the action will be invoked when a worker thread becomes available..</summary>
        /// <param name="action">The action to be invoked on a worker thread.</param>
        public static void InvokeOnWorkerThread(Action action)
        {
            _ = action.WhenNotNull(nameof(action));

            if (GetIsBound(out _))
            {
                ThreadPool.QueueUserWorkItem(_ => action(), null);
            }
            else
            {
                action();
            }
        }

        private static bool GetIsBound(out Dispatcher dispatcher)
        {
            dispatcher = Application.Current?.Dispatcher;

            Throw<InvalidOperationException>.WhenNull(dispatcher, "The application's dispatcher is not available.");
            
            return dispatcher.CheckAccess();
        }
    }
}