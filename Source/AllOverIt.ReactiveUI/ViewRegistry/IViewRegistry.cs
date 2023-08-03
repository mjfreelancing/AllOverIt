using AllOverIt.ReactiveUI.ViewRegistry.Events;
using System;
using System.Collections.Generic;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    // Can enumerate over all views for all model types
    public interface IViewRegistry<TViewId> : IEnumerable<ViewModelViewItem<TViewId>>
    {
        event ViewRegistryEventHandler OnUpdate;

        int GetViewCountFor<TViewModel>() where TViewModel : class;

        int GetViewCountFor(Type viewModelType);

        IReadOnlyCollection<Type> GetViewModelTypes();

        IReadOnlyCollection<ViewItem<TViewId>> GetViewsFor<TViewModel>() where TViewModel : class;

        IReadOnlyCollection<ViewItem<TViewId>> GetViewsFor(Type viewModelType);

        void CreateOrActivateFor<TViewModel>(int maxCount, Func<IReadOnlyCollection<ViewItem<TViewId>>, TViewId> nextViewId,
            Action<TViewModel, TViewId> configure = default) where TViewModel : class;

        bool TryCloseAllViews();
    }
}