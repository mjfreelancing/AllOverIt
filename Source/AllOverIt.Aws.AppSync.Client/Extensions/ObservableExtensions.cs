using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AllOverIt.Aws.AppSync.Client.Extensions
{
    internal static class ObservableExtensions
    {
        internal static Task<TResult> WaitUntilAsync<TResult>(
            this IObservable<TResult> observable,
            Func<TResult, bool> predicate,
            Func<TResult, Task<TResult>> action)
        {
            return WaitUntilAsync<TResult, TResult>(observable, predicate, action);
        }

        internal static async Task<TResult> WaitUntilAsync<TType, TResult>(
            this IObservable<TType> observable,
            Func<TType, bool> predicate,
            Func<TType, Task<TResult>> action)
        {
            IDisposable subscription = null;

            try
            {
                var tcs = new TaskCompletionSource<TResult>();

                subscription = observable
                    .TakeUntil(predicate)
                    .LastAsync()
                    .SelectMany(action.Invoke)
                    .Subscribe(
                        state =>
                        {
                            tcs.SetResult(state);
                        },
                        exception =>
                        {
                            tcs.SetException(exception);
                        },
                        () => { });

                // If there was an exception it will be re-thrown as we cannot return a result
                return await tcs.Task;
            }
            finally
            {
                subscription?.Dispose();
            }
        }
    }
}