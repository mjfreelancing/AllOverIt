using ReactiveUI;

namespace AllOverIt.ReactiveUI.Factories
{
    public interface IViewFactory
    {
        IViewFor<TViewModel> CreateViewFor<TViewModel>() where TViewModel : class;
    }
}