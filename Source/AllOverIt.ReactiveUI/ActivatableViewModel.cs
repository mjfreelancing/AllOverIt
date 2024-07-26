using AllOverIt.Assertion;
using AllOverIt.ReactiveUI.Factories;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace AllOverIt.ReactiveUI
{
    /// <summary>Provides a base class implementation of <see cref="IActivatableViewModel"/> to indicate the ViewModel
    /// is interested in Activation events.</summary>
    public abstract class ActivatableViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly Subject<bool> _deactivating = new();

        /// <inheritdoc />
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        /// <summary>Constructor.</summary>
        protected ActivatableViewModel()
        {
            this.WhenActivated(disposables =>
            {
                OnActivated(disposables);

                Disposable.Create(OnDeactivated).DisposeWith(disposables);
            });
        }

        // https://www.reactiveui.net/docs/guidelines/framework/dispose-your-subscriptions
        // Not all subscriptions need to be disposed. It's like events. If a component exposes an event and also subscribes to it itself,
        // it doesn't need to unsubscribe. That's because the subscription is manifested as the component having a reference to itself.
        // Same is true with Rx. If you're a VM and you e.g. WhenAnyValue against your own property, there's no need to clean that up because
        // that is manifested as the VM having a reference to itself.

        /// <summary>Override this in the concrete ViewModel to be notified when it is activated.</summary>
        /// <param name="disposables">Used to collate all the disposables to be cleaned up during deactivation.</param>
        protected abstract void OnActivated(CompositeDisposable disposables);

        /// <summary>Override this in the concrete ViewModel to be notified when it is deactivated.</summary>
        protected virtual void OnDeactivated()
        {
            _deactivating.OnNext(true);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that will be automatically cancelled, if running. when <see cref="OnDeactivated"/> is called.</summary>
        /// <summary>Creates a <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that will be automatically cancelled, if running, when <see cref="OnDeactivated"/> is called.</summary>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that will be automatically cancelled, if running. when <see cref="OnDeactivated"/> is called.</returns>
        protected ReactiveCommand<Unit, Unit> CreateAutoCancellingCommand(Func<CancellationToken, Task> action, IObservable<bool>? canExecute = default,
        /// <returns>A <c>ReactiveCommand&lt;Unit, Unit&gt;</c> that will be automatically cancelled, if running, when <see cref="OnDeactivated"/> is called.</returns>
        protected ReactiveCommand<Unit, Unit> CreateAutoCancellingCommand(Func<CancellationToken, Task> action, IObservable<bool> canExecute = default,
            IScheduler outputScheduler = default)
        {
            _ = action.WhenNotNull(nameof(action));

            return CommandFactory.CreateCancellableCommand(action, () => _deactivating, canExecute, outputScheduler);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;Unit, TResult&gt;</c> that will be automatically cancelled, if running. when <see cref="OnDeactivated"/> is called.</summary>
        /// <summary>Creates a <c>ReactiveCommand&lt;Unit, TResult&gt;</c> that will be automatically cancelled, if running, when <see cref="OnDeactivated"/> is called.</summary>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <c>ReactiveCommand&lt;Unit, TResult&gt;</c> that will be automatically cancelled, if running. when <see cref="OnDeactivated"/> is called.</returns>
        /// <returns>A <c>ReactiveCommand&lt;Unit, TResult&gt;</c> that will be automatically cancelled, if running, when <see cref="OnDeactivated"/> is called.</returns>
        protected ReactiveCommand<Unit, TResult> CreateAutoCancellingCommand<TResult>(Func<CancellationToken, Task<TResult>> action,
            IObservable<bool> canExecute = default, IScheduler outputScheduler = default)
        {
            _ = action.WhenNotNull(nameof(action));

            return CommandFactory.CreateCancellableCommand(action, () => _deactivating, canExecute, outputScheduler);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;TParam, Unit&gt;</c> that will be automatically cancelled, if running, when <see cref="OnDeactivated"/> is called.</summary>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <c>ReactiveCommand&lt;TParam, Unit&gt;</c> that will be automatically cancelled, if running, when <see cref="OnDeactivated"/> is called.</returns>
        protected ReactiveCommand<TParam, Unit> CreateAutoCancellingCommand<TParam>(Func<TParam, CancellationToken, Task> action,
            IObservable<bool> canExecute = default, IScheduler outputScheduler = default)
        {
            _ = action.WhenNotNull(nameof(action));

            return CommandFactory.CreateCancellableCommand(action, () => _deactivating, canExecute, outputScheduler);
        }

        /// <summary>Creates a <c>ReactiveCommand&lt;TParam, TResult&gt;</c> that will be automatically cancelled, if running, when <see cref="OnDeactivated"/> is called.</summary>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="canExecute">An optional observable that determines if the command can be executed.</param>
        /// <param name="outputScheduler">An optional scheduler that is used to surface events. Defaults to <c>RxApp.MainThreadScheduler</c>.</param>
        /// <returns>A <c>ReactiveCommand&lt;TParam, TResult&gt;</c> that will be automatically cancelled, if running, when <see cref="OnDeactivated"/> is called.</returns>
        protected ReactiveCommand<TParam, TResult> CreateAutoCancellingCommand<TParam, TResult>(Func<TParam, CancellationToken, Task<TResult>> action,
            IObservable<bool> canExecute = default, IScheduler outputScheduler = default)
        {
            _ = action.WhenNotNull(nameof(action));

            return CommandFactory.CreateCancellableCommand(action, () => _deactivating, canExecute, outputScheduler);
        }
    }
}