using System.Reactive.Disposables;
using ReactiveUI;

namespace AllOverIt.ReactiveUI
{
    public abstract class ActivatableViewModel : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

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
        protected abstract void OnActivated(CompositeDisposable disposables);

        protected virtual void OnDeactivated()
        {
        }
    }
}