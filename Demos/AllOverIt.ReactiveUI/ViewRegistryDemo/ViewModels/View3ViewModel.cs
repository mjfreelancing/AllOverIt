using AllOverIt.ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;

namespace ViewRegistryDemo.ViewModels
{
    public sealed class View3ViewModel : ActivatableViewModel
    {
        [Reactive]
        public int Id { get; set; }

        protected override void OnActivated(CompositeDisposable disposables)
        {

        }
    }
}
