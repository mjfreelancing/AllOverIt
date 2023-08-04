using AllOverIt.ReactiveUI.ViewRegistry.Events;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    /// <summary>Represents a registry of views (typically windows) associated with a view model type. The views are expected
    /// to implement <see cref="IViewFor{T}"/>, where <c>T</c> is any view model type. The registry is enumerable to provide
    /// easy access to each of the currently registered view models and associated views.</summary>
    /// <typeparam name="TViewId"></typeparam>
    public interface IViewRegistry<TViewId> : IEnumerable<ViewModelViewItem<TViewId>>
    {
        /// <summary>An event raised when a view is added to or removed from the registry.</summary>
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