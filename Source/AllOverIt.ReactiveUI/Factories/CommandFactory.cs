#nullable enable

using AllOverIt.Assertion;
using AllOverIt.Extensions;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace AllOverIt.ReactiveUI.Factories
{
    /// <summary>Provides factory methods that help with the creation of specialized reactive commands.</summary>
    public static class CommandFactory
    {
        /// <summary>Creates a <see cref="ReactiveCommand{Unit, Unit}"/> that can be cancelled when another 'cancel' <see cref="IObservable{Unit}"/>,
        /// such as another <c>ReactiveCommand</c>, emits a value.</summary>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="cancelObservable"></param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <see cref="ReactiveCommand{Unit, Unit}"/> that can be cancelled when another 'cancel' observable, such as another
        /// <c>ReactiveCommand</c>, emits a value.</returns>
        public static ReactiveCommand<Unit, Unit> CreateCancellableCommand(Func<CancellationToken, Task<Unit>> action,
            Func<IObservable<Unit>> cancelObservable, IObservable<bool>? canExecute = default, IScheduler? outputScheduler = default)
        {
            _ = action.WhenNotNull(nameof(action));
            _ = cancelObservable.WhenNotNull(nameof(cancelObservable));

            return CreateCancellableCommand<Unit>(action, cancelObservable, canExecute, outputScheduler);
        }

        /// <summary>Creates a <see cref="ReactiveCommand{Unit, TResult}"/> that can be cancelled when another 'cancel' <see cref="IObservable{Unit}"/>,
        /// such as another <c>ReactiveCommand</c>, emits a value.</summary>
        /// <typeparam name="TResult">The command's result type.</typeparam>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="cancelObservable">The observable that will cancel the command when it emits a value.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <see cref="ReactiveCommand{Unit, TResult}"/> that can be cancelled when another 'cancel' <see cref="IObservable{Unit}"/>,
        /// such as another <c>ReactiveCommand</c>, emits a value.</returns>
        public static ReactiveCommand<Unit, TResult> CreateCancellableCommand<TResult>(Func<CancellationToken, Task<TResult>> action,
            Func<IObservable<Unit>> cancelObservable, IObservable<bool>? canExecute = default, IScheduler? outputScheduler = default)
        {
            _ = action.WhenNotNull(nameof(action));
            _ = cancelObservable.WhenNotNull(nameof(cancelObservable));

            return ReactiveCommand
               .CreateFromObservable<Unit, TResult>(
                    _ => Observable.StartAsync(token => action.Invoke(token)).TakeUntil(cancelObservable.Invoke()),
                    canExecute, outputScheduler);
        }

        /// <summary>Creates a <see cref="ReactiveCommand{TParam, TResult}"/> that can be cancelled when another 'cancel' <see cref="IObservable{Unit}"/>,
        /// such as another <c>ReactiveCommand</c>, emits a value.</summary>
        /// <typeparam name="TParam">The command's input type.</typeparam>
        /// <typeparam name="TResult">The command's result type.</typeparam>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="cancelObservable">The observable that will cancel the command when it emits a value.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <see cref="ReactiveCommand{TParam, TResult}"/> that can be cancelled when another 'cancel' <see cref="IObservable{Unit}"/>,
        /// such as another <c>ReactiveCommand</c>, emits a value.</returns>
        public static ReactiveCommand<TParam, TResult> CreateCancellableCommand<TParam, TResult>(Func<TParam, CancellationToken, Task<TResult>> action,
            Func<IObservable<Unit>> cancelObservable, IObservable<bool>? canExecute = default, IScheduler? outputScheduler = default)
        {
            _ = action.WhenNotNull(nameof(action));
            _ = cancelObservable.WhenNotNull(nameof(cancelObservable));

            return CreateCancellableCommand<TParam, TResult, Unit>(action, cancelObservable, canExecute, outputScheduler);
        }


        /// <summary>Creates a <see cref="ReactiveCommand{TParam, TResult}"/> that can be cancelled when another 'cancel' <see cref="IObservable{TCancelResult}"/>,
        /// such as another <c>ReactiveCommand</c>, emits a value.</summary>
        /// <typeparam name="TParam">The command's input type.</typeparam>
        /// <typeparam name="TResult">The command's result type.</typeparam>
        /// <typeparam name="TCancelResult">The type emitted by the observable used to cancel the command.</typeparam>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="cancelObservable">The observable that will cancel the command when it emits a value.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <see cref="ReactiveCommand{TParam, TResult}"/> that can be cancelled when another 'cancel' <see cref="IObservable{TCancelResult}"/>,
        /// such as another <c>ReactiveCommand</c>, emits a value.</returns>
        public static ReactiveCommand<TParam, TResult> CreateCancellableCommand<TParam, TResult, TCancelResult>(Func<TParam, CancellationToken, Task<TResult>> action,
            Func<IObservable<TCancelResult>> cancelObservable, IObservable<bool>? canExecute = default, IScheduler? outputScheduler = default)
        {
            _ = action.WhenNotNull(nameof(action));
            _ = cancelObservable.WhenNotNull(nameof(cancelObservable));

            return ReactiveCommand
               .CreateFromObservable<TParam, TResult>(
                    param => Observable.StartAsync(token => action.Invoke(param, token)).TakeUntil(cancelObservable.Invoke()),
                    canExecute, outputScheduler);
        }

        /// <summary>Creates a <see cref="ReactiveCommand{Unit, Unit}"/> that can be used to cancel one or more other commands.</summary>
        /// <param name="canExecutes">One or more observables that, individually, determine if the returned command can be executed.</param>
        /// <returns>A <see cref="ReactiveCommand{Unit, Unit}"/> that can be used to cancel one or more other commands.</returns>
        public static ReactiveCommand<Unit, Unit> CreateCancelCommand(params IObservable<bool>[] canExecutes)
        {
            _ = canExecutes.WhenNotNullOrEmpty(nameof(canExecutes), errorMessage: "At least one observable is required.");

            var obs = canExecutes.SelectToArray(canExecute => canExecute.StartWith(false));

            var canExecute = obs.Length switch
            {
                1 => obs[0],

                2 => Observable.CombineLatest(
                        obs[0], obs[1],
                        (o1, o2) => o1 || o2),

                3 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2],
                        (o1, o2, o3) => o1 || o2 || o3),

                4 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3],
                        (o1, o2, o3, o4) => o1 || o2 || o3 || o4),

                5 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3], obs[4],
                        (o1, o2, o3, o4, o5) => o1 || o2 || o3 || o4 || o5),

                6 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3], obs[4], obs[5],
                        (o1, o2, o3, o4, o5, o6) => o1 || o2 || o3 || o4 || o5 || o6),

                7 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3], obs[4], obs[5], obs[6],
                        (o1, o2, o3, o4, o5, o6, o7) => o1 || o2 || o3 || o4 || o5 || o6 || o7),

                8 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3], obs[4], obs[5], obs[6], obs[7],
                        (o1, o2, o3, o4, o5, o6, o7, o8) => o1 || o2 || o3 || o4 || o5 || o6 || o7 || o8),

                9 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3], obs[4], obs[5], obs[6], obs[7], obs[8],
                        (o1, o2, o3, o4, o5, o6, o7, o8, o9) => o1 || o2 || o3 || o4 || o5 || o6 || o7 || o8 || o9),

                10 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3], obs[4], obs[5], obs[6], obs[7], obs[8], obs[9],
                        (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10) => o1 || o2 || o3 || o4 || o5 || o6 || o7 || o8 || o9 || o10),

                11 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3], obs[4], obs[5], obs[6], obs[7], obs[8], obs[9], obs[10],
                        (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11) => o1 || o2 || o3 || o4 || o5 || o6 || o7 || o8 || o9 || o10 || o11),

                12 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3], obs[4], obs[5], obs[6], obs[7], obs[8], obs[9], obs[10], obs[11],
                        (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12) => o1 || o2 || o3 || o4 || o5 || o6 || o7 || o8 || o9 || o10 || o11 || o12),

                13 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3], obs[4], obs[5], obs[6], obs[7], obs[8], obs[9], obs[10], obs[11], obs[12],
                        (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13) => o1 || o2 || o3 || o4 || o5 || o6 || o7 || o8 || o9 || o10 || o11 || o12 || o13),

                14 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3], obs[4], obs[5], obs[6], obs[7], obs[8], obs[9], obs[10], obs[11], obs[12], obs[13],
                        (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14) => o1 || o2 || o3 || o4 || o5 || o6 || o7 || o8 || o9 || o10 || o11 || o12 || o13 || o14),

                15 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3], obs[4], obs[5], obs[6], obs[7], obs[8], obs[9], obs[10], obs[11], obs[12], obs[13], obs[14],
                        (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14, o15) => o1 || o2 || o3 || o4 || o5 || o6 || o7 || o8 || o9 || o10 || o11 || o12 || o13 || o14 || o15),

                16 => Observable.CombineLatest(
                        obs[0], obs[1], obs[2], obs[3], obs[4], obs[5], obs[6], obs[7], obs[8], obs[9], obs[10], obs[11], obs[12], obs[13], obs[14], obs[15],
                        (o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14, o15, o16) => o1 || o2 || o3 || o4 || o5 || o6 || o7 || o8 || o9 || o10 || o11 || o12 || o13 || o14 || o15 || o16),

                _ => throw new ArgumentException("A maximum of 16 observables per cancel command is supported.")
            };

            return ReactiveCommand.Create(() => { }, canExecute);
        }

        /// <summary>Creates a <see cref="ReactiveCommand{Unit, Unit}"/> that can be used to cancel one or more other commands.</summary>
        /// <param name="cancellableCommands">One or more commands created by one of the <c>CreateCancellableCommand()</c> overloads.</param>
        /// <returns>A <see cref="ReactiveCommand{Unit, Unit}"/> that can be used to cancel one or more other commands.</returns>
        public static ReactiveCommand<Unit, Unit> CreateCancelCommand(params IReactiveCommand[] cancellableCommands)
        {
            _ = cancellableCommands.WhenNotNullOrEmpty(nameof(cancellableCommands), errorMessage: "At least one cancellable command is required.");

            return cancellableCommands.Length == 1
                ? ReactiveCommand.Create(() => { }, cancellableCommands[0].IsExecuting)
                : CreateCancelCommand([.. cancellableCommands.Select(command => command.IsExecuting)]);
        }
    }
}