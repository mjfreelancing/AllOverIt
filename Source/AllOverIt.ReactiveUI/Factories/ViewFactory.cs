using AllOverIt.Assertion;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;

namespace AllOverIt.ReactiveUI.Factories
{
    internal sealed class ViewFactory : IViewFactory
    {
        private readonly IServiceProvider _provider;

        public ViewFactory(IServiceProvider provider)
        {
            _provider = provider.WhenNotNull(nameof(provider));
        }

        public IViewFor<TViewModel> CreateViewFor<TViewModel>() where TViewModel : class
        {
            return _provider.GetService<IViewFor<TViewModel>>();
        }
    }
}