using ReactiveUI;

namespace AllOverIt.ReactiveUI.Factories
{
    /// <summary>Represents an interface to instantiate a View based on its' associated ViewModel.</summary>
    public interface IViewFactory
    {
        /// <summary>Instantiates a View based on a ViewModel.</summary>
        /// <typeparam name="TViewModel">The ViewModel type associated with the View to create, previously registered as <see cref="IViewFor{TViewModel}"/>.</typeparam>
        /// <returns>The instantiated view.</returns>
        IViewFor<TViewModel> CreateViewFor<TViewModel>() where TViewModel : class;
    }
}