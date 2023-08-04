using AllOverIt.ReactiveUI.ViewRegistry.Events;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    /// <summary>Represents a registry of views (typically windows) associated with a view model type. The views are expected
    /// to implement <see cref="IViewFor{T}"/>, where <c>T</c> is any view model type. The registry is enumerable to provide
    /// easy access to each of the currently registered view models and associated views.</summary>
    /// <typeparam name="TViewId">The type used for identifying each view within the registry.</typeparam>
    public interface IViewRegistry<TViewId> : IEnumerable<ViewModelViewItem<TViewId>>
    {
        /// <summary>An event raised when a view is added to or removed from the registry.</summary>
        event ViewRegistryEventHandler OnUpdate;

        /// <summary>Indicates if the registry is empty.</summary>
        bool IsEmpty { get; }

        /// <summary>Gets the count of registered views associated with the <typeparamref name="TViewModel"/> type.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <returns>The count of registered views associated with the <typeparamref name="TViewModel"/> type.</returns>
        int GetViewCountFor<TViewModel>() where TViewModel : class;

        /// <summary>Gets the count of registered views associated with the <see cref="Type"/> specified by <paramref name="viewModelType"/>.</summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <returns>The count of registered views associated with the <see cref="Type"/> specified by <paramref name="viewModelType"/>.</returns>
        int GetViewCountFor(Type viewModelType);

        /// <summary>Gets the currently registered view model types.</summary>
        /// <returns>A readonly collection of the currently registered view model types.</returns>
        IReadOnlyCollection<Type> GetViewModelTypes();

        /// <summary>Gets a readonly collection of all currently registered views associated with the <typeparamref name="TViewModel"/> type.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <returns>A readonly collection of all currently registered views associated with the <typeparamref name="TViewModel"/> type.</returns>
        IReadOnlyCollection<ViewItem<TViewId>> GetViewsFor<TViewModel>() where TViewModel : class;

        /// <summary>Gets a readonly collection of all currently registered views associated with the <see cref="Type"/> specified by <paramref name="viewModelType"/>.</summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <returns>A readonly collection of all currently registered views associated with the <see cref="Type"/> specified by <paramref name="viewModelType"/>.</returns>
        IReadOnlyCollection<ViewItem<TViewId>> GetViewsFor(Type viewModelType);

        /// <summary>Creates a new view or activates existing views associated with the <typeparamref name="TViewModel"/> type. A new
        /// view will be created if the number of views for the specified <typeparamref name="TViewModel"/> type is fewer than
        /// <paramref name="maxCount"/>. If the number of views in the registry equals, or exceeds, <paramref name="maxCount"/> then
        /// the existing views will be activated to bring them to the foreground. The view will be automatically de-registered from the 
        /// registry when it is closed.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="maxCount">The maximum number of views allowed for the <typeparamref name="TViewModel"/> type. The registry doesn't
        /// enforce this value between calls; it is only used to compare against the number of views already present in the registry.</param>
        /// <param name="nextViewId">A callback that allows the caller to provide an identifier for each newly created view. The callback provides
        /// a readonly collection of all currently registered views for the specified <typeparamref name="TViewModel"/> type.</param>
        /// <param name="configure">An optional callback that allows the caller to configure the view model associated with each newly
        /// created view. The callback also includes the the Id provided by the <paramref name="nextViewId"/> callback.</param>
        void CreateOrActivateFor<TViewModel>(int maxCount, Func<IReadOnlyCollection<ViewItem<TViewId>>, TViewId> nextViewId,
            Action<TViewModel, TViewId> configure = default) where TViewModel : class;

        /// <summary>Attempts to close all currently registered views. Views that cancel being closed will remain registered.</summary>
        /// <returns><see langword="True"/> if all views were closed, otherwise <see langword="False"/>.</returns>
        bool TryCloseAllViews();
    }
}