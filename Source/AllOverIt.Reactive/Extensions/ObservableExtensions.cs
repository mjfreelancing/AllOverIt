using AllOverIt.Assertion;
using System.Reactive.Linq;

namespace AllOverIt.Reactive.Extensions
{
    /// <summary>Provides extension methods for <see cref="IObservable{T}"/>.</summary>
    public static class ObservableExtensions
    {
        /// <summary>Subscribes to an observable and waits for a specified condition to be satisfied before returning a result.</summary>
        /// <typeparam name="TResult">The type being observed and returned from the observable.</typeparam>
        /// <param name="observable">The observable to wait for.</param>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <param name="action">An optional action to apply to the result before returning it.</param>
        /// <returns>The result of the observable.</returns>
        public static IObservable<TResult> WaitUntil<TResult>(
            this IObservable<TResult> observable,
            Func<TResult, bool> predicate,
            Func<TResult, TResult>? action = default)
        {
            _ = observable.WhenNotNull(nameof(observable));
            _ = predicate.WhenNotNull(nameof(predicate));

            action ??= result => result;

            return observable.WaitUntil<TResult, TResult>(predicate, action);
        }

        /// <summary>Subscribes to an observable and waits for a specified condition to be satisfied before returning a projected result.</summary>
        /// <typeparam name="TType">The type being observed.</typeparam>
        /// <typeparam name="TResult">The projected result type of the observable.</typeparam>
        /// <param name="observable">The observable to wait for.</param>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <param name="action">An action to apply to the observable result so it can be projected to another result type.</param>
        /// <returns>The projected result of the observable.</returns>
        public static IObservable<TResult> WaitUntil<TType, TResult>(
            this IObservable<TType> observable,
            Func<TType, bool> predicate,
            Func<TType, TResult> action)
        {
            _ = observable.WhenNotNull(nameof(observable));
            _ = predicate.WhenNotNull(nameof(predicate));
            _ = action.WhenNotNull(nameof(action));

            return observable
                .TakeUntil(predicate.Invoke)
                .LastAsync()
                .Select(action.Invoke);
        }

        /// <summary>Subscribes to an observable and waits for a specified condition to be satisfied before returning a result.</summary>
        /// <typeparam name="TResult">The type being observed and returned from the observable.</typeparam>
        /// <param name="observable">The observable to wait for.</param>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <param name="action">An optional action to apply to the result before returning it.</param>
        /// <returns>The result of the observable.</returns>
        public static Task<TResult> WaitUntilAsync<TResult>(
            this IObservable<TResult> observable,
            Func<TResult, bool> predicate,
            Func<TResult, Task<TResult>>? action = default)
        {
            _ = observable.WhenNotNull(nameof(observable));
            _ = predicate.WhenNotNull(nameof(predicate));

            action ??= Task.FromResult;

            return observable.WaitUntilAsync<TResult, TResult>(predicate, action);
        }

        /// <summary>Subscribes to an observable and waits for a specified condition to be satisfied before returning a projected result.</summary>
        /// <typeparam name="TType">The type being observed.</typeparam>
        /// <typeparam name="TResult">The projected result type of the observable.</typeparam>
        /// <param name="observable">The observable to wait for.</param>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <param name="action">An action to apply to the observable result so it can be projected to another result type.</param>
        /// <returns>The projected result of the observable.</returns>
        public static async Task<TResult> WaitUntilAsync<TType, TResult>(
            this IObservable<TType> observable,
            Func<TType, bool> predicate,
            Func<TType, Task<TResult>> action)
        {
            _ = observable.WhenNotNull(nameof(observable));
            _ = predicate.WhenNotNull(nameof(predicate));
            _ = action.WhenNotNull(nameof(action));

            var tcs = new TaskCompletionSource<TResult>();

            var subscription = observable
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

            using (subscription)
            {
                // If there was an exception it will be re-thrown as we cannot return a result
                return await tcs.Task;
            }
        }
    }
}