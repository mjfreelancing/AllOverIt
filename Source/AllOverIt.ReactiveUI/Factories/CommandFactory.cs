using AllOverIt.Assertion;
using AllOverIt.Extensions;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace AllOverIt.ReactiveUI.Factories
{
    /*
            Has Input   Has Result  Custom Cancel   Param       Result      Cancel Type     Signature
            =========   ==========  =============   =====       ======      ===========     ===========================================================================================================================================================================================
            No          No          No              Unit        Unit        Unit            ReactiveCommand<Unit, Unit> CreateCancellableCommand(Func<CancellationToken, Task> action, Func<IObservable<Unit>> cancelObservable) { }
            No          No          Yes             Unit        Unit        TCancel         ReactiveCommand<Unit, Unit> CreateCancellableCommand<TCancel>(Func<CancellationToken, Task> action, Func<IObservable<TCancel>> cancelObservable) { }
            No          Yes         No              Unit        TResult     Unit            ReactiveCommand<Unit, TResult> CreateCancellableCommand<TResult>(Func<CancellationToken, Task<TResult>> action, Func<IObservable<Unit>> cancelObservable) { }
            No          Yes         Yes             Unit        TResult     TCancel         ReactiveCommand<Unit, TResult> CreateCancellableCommand<TResult, TCancel>(Func<CancellationToken, Task<TResult>> action, Func<IObservable<TCancel>> cancelObservable) { }
            Yes         No          No              TParam      Unit        Unit            ReactiveCommand<TParam, Unit> CreateCancellableCommand<TParam>(Func<TParam, CancellationToken, Task> action, Func<IObservable<Unit>> cancelObservable) { }
            Yes         No          Yes             TParam      Unit        TCancel         ReactiveCommand<TParam, Unit> CreateCancellableCommand<TParam, TCancel>(Func<TParam, CancellationToken, Task> action, Func<IObservable<TCancel>> cancelObservable) { }
            Yes         Yes         No              TParam      TResult     Unit            ReactiveCommand<TParam, TResult> CreateCancellableCommand<TParam, TResult>(Func<TParam, CancellationToken, Task<TResult>> action, Func<IObservable<Unit>> cancelObservable) { }
            Yes         Yes         Yes             TParam      TResult     TCancel         ReactiveCommand<TParam, TResult> CreateCancellableCommand<TParam, TResult, TCancel>(Func<TParam, CancellationToken, Task<TResult>> action, Func<IObservable<TCancel>> cancelObservable) { }
     */

    /// <summary>Provides factory methods that help with the creation of specialized reactive commands.</summary>
    public static class CommandFactory
    {
        /// <summary>Creates a <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that can be cancelled when an <c>IObservable&lt;Unit&gt;</c> emits a value.</summary>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="cancelObservable">The observable that will cancel the command when it emits a value.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that can be cancelled when an <c>IObservable&lt;Unit&gt;</c> emits a value.</returns>
        public static ReactiveCommand<Unit, Unit> CreateCancellableCommand(Func<CancellationToken, Task> action, Func<IObservable<Unit>> cancelObservable,
            IObservable<bool>? canExecute = default, IScheduler? outputScheduler = default)
        {
            _ = action.WhenNotNull();
            _ = cancelObservable.WhenNotNull();

            return CreateCancellableCommand<Unit>(action, cancelObservable, canExecute, outputScheduler);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that can be cancelled when an <c>IObservable&lt;TCancel&gt;</c> emits a value.</summary>
        /// <typeparam name="TCancel">The value type emitted by <paramref name="cancelObservable"/>.</typeparam>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="cancelObservable">The observable that will cancel the command when it emits a value.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that can be cancelled when an <c>IObservable&lt;TCancel&gt;</c> emits a value.</returns>
        public static ReactiveCommand<Unit, Unit> CreateCancellableCommand<TCancel>(Func<CancellationToken, Task> action, Func<IObservable<TCancel>> cancelObservable,
            IObservable<bool>? canExecute = default, IScheduler? outputScheduler = default)
        {
            _ = action.WhenNotNull(nameof(action));
            _ = cancelObservable.WhenNotNull(nameof(cancelObservable));

            return ReactiveCommand
               .CreateFromObservable<Unit, Unit>(
                    _ => Observable.StartAsync(async token =>
                    {
                        await action.Invoke(token).ConfigureAwait(false);

                        return Unit.Default;
                    }).TakeUntil(cancelObservable.Invoke()),
                    canExecute, outputScheduler);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;Unit, TResult&gt;</c> that can be cancelled when an <c>IObservable&lt;Unit&gt;</c> emits a value.</summary>
        /// <typeparam name="TResult">The result type returned by the command.</typeparam>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="cancelObservable">The observable that will cancel the command when it emits a value.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <c>ReactiveCommand&lt;Unit, TResult&gt;</c> that can be cancelled when an <c>IObservable&lt;Unit&gt;</c> emits a value.</returns>
        public static ReactiveCommand<Unit, TResult> CreateCancellableCommand<TResult>(Func<CancellationToken, Task<TResult>> action, Func<IObservable<Unit>> cancelObservable,
            IObservable<bool>? canExecute = default, IScheduler? outputScheduler = default)
        {
            _ = action.WhenNotNull(nameof(action));
            _ = cancelObservable.WhenNotNull(nameof(cancelObservable));

            return CreateCancellableCommand<TResult, Unit>(action, cancelObservable, canExecute, outputScheduler);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;Unit, TResult&gt;</c> that can be cancelled when an <c>IObservable&lt;TCancel&gt;</c> emits a value.</summary>
        /// <typeparam name="TResult">The result type returned by the command.</typeparam>
        /// <typeparam name="TCancel">The type emitted by the observable used to cancel the command.</typeparam>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="cancelObservable">The observable that will cancel the command when it emits a value.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <c>ReactiveCommand&lt;Unit, TResult&gt;</c> that can be cancelled when an <c>IObservable&lt;TCancel&gt;</c> emits a value.</returns>
        public static ReactiveCommand<Unit, TResult> CreateCancellableCommand<TResult, TCancel>(Func<CancellationToken, Task<TResult>> action, Func<IObservable<TCancel>> cancelObservable,
            IObservable<bool>? canExecute = default, IScheduler? outputScheduler = default)
        {
            _ = action.WhenNotNull();
            _ = cancelObservable.WhenNotNull();

            return ReactiveCommand
               .CreateFromObservable<Unit, TResult>(
                    _ => Observable.StartAsync(action).TakeUntil(cancelObservable.Invoke()),
                    canExecute, outputScheduler);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;TParam, Unit&gt;</c> that can be cancelled when an <c>IObservable&lt;Unit&gt;</c> emits a value.</summary>
        /// <typeparam name="TParam">The parameter type forwarded to the command.</typeparam>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="cancelObservable">The observable that will cancel the command when it emits a value.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <c>ReactiveCommand&lt;TParam, Unit&gt;</c> that can be cancelled when an <c>IObservable&lt;Unit&gt;</c> emits a value.</returns>
        public static ReactiveCommand<TParam, Unit> CreateCancellableCommand<TParam>(Func<TParam, CancellationToken, Task> action, Func<IObservable<Unit>> cancelObservable,
            IObservable<bool>? canExecute = default, IScheduler? outputScheduler = default)
        {
            _ = action.WhenNotNull(nameof(action));
            _ = cancelObservable.WhenNotNull(nameof(cancelObservable));

            return CreateCancellableCommand<TParam, Unit>(action, cancelObservable, canExecute, outputScheduler);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;TParam, Unit&gt;</c> that can be cancelled when an <c>IObservable&lt;TCancel&gt;</c> emits a value.</summary>
        /// <typeparam name="TParam">The parameter type forwarded to the command.</typeparam>
        /// <typeparam name="TCancel">The value type emitted by <paramref name="cancelObservable"/>.</typeparam>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="cancelObservable">The observable that will cancel the command when it emits a value.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <c>ReactiveCommand&lt;TParam, Unit&gt;</c> that can be cancelled when an <c>IObservable&lt;TCancel&gt;</c> emits a value.</returns>
        public static ReactiveCommand<TParam, Unit> CreateCancellableCommand<TParam, TCancel>(Func<TParam, CancellationToken, Task> action, Func<IObservable<TCancel>> cancelObservable,
            IObservable<bool>? canExecute = default, IScheduler? outputScheduler = default)
        {
            _ = action.WhenNotNull(nameof(action));
            _ = cancelObservable.WhenNotNull(nameof(cancelObservable));

            return ReactiveCommand
               .CreateFromObservable<TParam, Unit>(
                    param => Observable.StartAsync(async token =>
                    {
                        await action.Invoke(param, token).ConfigureAwait(false);

                        return Unit.Default;
                    }).TakeUntil(cancelObservable.Invoke()),
                    canExecute, outputScheduler);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;TParam, TResult&gt;</c> that can be cancelled when an <c>IObservable&lt;Unit&gt;</c> emits a value.</summary>
        /// <typeparam name="TParam">The parameter type forwarded to the command.</typeparam>
        /// <typeparam name="TResult">The result type returned by the command.</typeparam>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="cancelObservable">The observable that will cancel the command when it emits a value.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <c>ReactiveCommand&lt;TParam, TResult&gt;</c> that can be cancelled when an <c>IObservable&lt;Unit&gt;</c> emits a value.</returns>
        public static ReactiveCommand<TParam, TResult> CreateCancellableCommand<TParam, TResult>(Func<TParam, CancellationToken, Task<TResult>> action, Func<IObservable<Unit>> cancelObservable,
            IObservable<bool>? canExecute = default, IScheduler? outputScheduler = default)
        {
            _ = action.WhenNotNull();
            _ = cancelObservable.WhenNotNull();

            return CreateCancellableCommand<TParam, TResult, Unit>(action, cancelObservable, canExecute, outputScheduler);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;TParam, TResult&gt;</c> that can be cancelled when an <c>IObservable&lt;TCancel&gt;</c> emits a value.</summary>
        /// <typeparam name="TParam">The parameter type forwarded to the command.</typeparam>
        /// <typeparam name="TResult">The result type returned by the command.</typeparam>
        /// <typeparam name="TCancel">The value type emitted by <paramref name="cancelObservable"/>.</typeparam>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="cancelObservable">The observable that will cancel the command when it emits a value.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <c>ReactiveCommand&lt;TParam, TResult&gt;</c> that can be cancelled when an <c>IObservable&lt;TCancel&gt;</c> emits a value.</returns>
        public static ReactiveCommand<TParam, TResult> CreateCancellableCommand<TParam, TResult, TCancel>(Func<TParam, CancellationToken, Task<TResult>> action, Func<IObservable<TCancel>> cancelObservable,
            IObservable<bool>? canExecute = default, IScheduler? outputScheduler = default)
        {
            _ = action.WhenNotNull();
            _ = cancelObservable.WhenNotNull();

            return ReactiveCommand
               .CreateFromObservable<TParam, TResult>(
                    param => Observable.StartAsync(token => action.Invoke(param, token)).TakeUntil(cancelObservable.Invoke()),
                    canExecute, outputScheduler);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that can be used to cancel one or more other commands.</summary>
        /// <param name="observables">One or more observables that, individually, determine if the returned command can be executed.</param>
        /// <returns>A <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that can be used to cancel one or more other commands.</returns>
        public static ReactiveCommand<Unit, Unit> CreateCancelCommand(params IObservable<bool>[] observables) => CreateCancelCommand(null, observables);

        /// <summary>Creates a <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that can be used to cancel one or more other commands.</summary>
        /// <param name="outputScheduler">The scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c> if <see langword="null"/>.</param>
        /// <param name="observables">One or more observables that, individually, determine if the returned command can be executed.</param>
        /// <returns>A <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that can be used to cancel one or more other commands.</returns>
        public static ReactiveCommand<Unit, Unit> CreateCancelCommand(IScheduler? outputScheduler, params IObservable<bool>[] observables)
        {
            _ = observables.WhenNotNullOrEmpty(errorMessage: "At least one observable is required.");

            var obs = observables.SelectToArray(canExecute => canExecute.StartWith(false));

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

                _ => throw new ArgumentOutOfRangeException(nameof(observables), "A maximum of 16 observables is supported.")
            };

            var scheduler = outputScheduler ?? RxApp.MainThreadScheduler;

            return ReactiveCommand.Create(() => { }, canExecute, scheduler);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that can be used to cancel one or more other commands.</summary>
        /// <param name="cancellableCommands">One or more commands created by one of the <c>CreateCancellableCommand()</c> overloads.</param>
        /// <returns>A <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that can be used to cancel one or more other commands.</returns>
        public static ReactiveCommand<Unit, Unit> CreateCancelCommand(params IReactiveCommand[] cancellableCommands)
            => CreateCancelCommand((IScheduler?)null, cancellableCommands);

        /// <summary>Creates a <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that can be used to cancel one or more other commands.</summary>
        /// <param name="outputScheduler">The scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c> if <see langword="null"/>.</param>
        /// <param name="cancellableCommands">One or more commands created by one of the <c>CreateCancellableCommand()</c> overloads.</param>
        /// <returns>A <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that can be used to cancel one or more other commands.</returns>
        public static ReactiveCommand<Unit, Unit> CreateCancelCommand(IScheduler? outputScheduler, params IReactiveCommand[] cancellableCommands)
        {
            _ = cancellableCommands.WhenNotNullOrEmpty(errorMessage: "At least one cancellable command is required.");

            var scheduler = outputScheduler ?? RxApp.MainThreadScheduler;

            return cancellableCommands.Length == 1
                ? ReactiveCommand.Create(() => { }, cancellableCommands[0].IsExecuting, scheduler)
                : CreateCancelCommand(outputScheduler, [.. cancellableCommands.Select(command => command.IsExecuting)]);
        }
    }
}