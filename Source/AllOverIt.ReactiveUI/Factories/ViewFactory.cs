using AllOverIt.Assertion;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;

namespace AllOverIt.ReactiveUI.Factories
{
    /// <summary>Instantiates a View based on its' associated ViewModel.</summary>
    internal sealed class ViewFactory : IViewFactory
    {
        private readonly IServiceProvider _provider;

        /// <summary>Constructor.</summary>
        /// <param name="provider">The service provider used to resolve Views from.</param>
        public ViewFactory(IServiceProvider provider)
        {
            _provider = provider.WhenNotNull(nameof(provider));
        }

        /// <inheritdoc />
        public IViewFor<TViewModel> CreateViewFor<TViewModel>() where TViewModel : class
        {
            return _provider.GetService<IViewFor<TViewModel>>();
        }
    }
}