using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AllOverIt.Aws.AppSync.Client.Extensions
{
    public static class ObservableExtensions
    {
        // If the exception handler is provided then the exception is re-thrown after the handler returns. If this is not
        // the desired behaviour, then handle the exception within the subscribed action.
        public static Task<TResult> WaitUntilAsync<TResult>(
            this IObservable<TResult> observable,
            Func<TResult, bool> predicate,
            Func<TResult, Task<TResult>> action,
            Action<Exception> exceptionHandler = null)
        {
            return WaitUntilAsync<TResult, TResult>(observable, predicate, action, exceptionHandler);
        }

        public static async Task<TResult> WaitUntilAsync<TType, TResult>(
            this IObservable<TType> observable,
            Func<TType, bool> predicate,
            Func<TType, Task<TResult>> action,
            Action<Exception> exceptionHandler = null)
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
                            exceptionHandler?.Invoke(exception);
                            tcs.SetException(exception);
                        },
                        () => { });

                return await tcs.Task;
            }
            finally
            {
                subscription?.Dispose();
            }
        }
    }
}