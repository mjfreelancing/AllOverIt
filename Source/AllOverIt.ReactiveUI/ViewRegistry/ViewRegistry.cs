using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.ReactiveUI.Factories;
using AllOverIt.ReactiveUI.ViewRegistry.Events;
using ReactiveUI;
using System.Collections;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    /// <summary>A registry of views (typically windows) associated with a view model type. The views are expected
    /// to implement <see cref="IViewFor{T}"/>, where <c>T</c> is any view model type. The registry is enumerable to provide
    /// easy access to each of the currently registered view models and associated views.</summary>
    /// <typeparam name="TViewId">The type used for identifying each view within the registry.</typeparam>
    public class ViewRegistry<TViewId> : IViewRegistry<TViewId> where TViewId : notnull
    {
        private class ViewRegistryItem
        {
            private readonly Type _viewModelType;
            private readonly IViewHandler _viewHandler;
            private readonly List<ViewItem<TViewId>> _viewItems = [];

            public event ViewRegistryEventHandler OnChange;     // raised when a view is added or removed

            public ViewItem<TViewId>[] Views => [.. _viewItems];

            public int ViewCount => _viewItems.Count;

            public ViewRegistryItem(Type viewModelType, IViewHandler viewHandler, ViewRegistryEventHandler onChangeHandler)
            {
                _viewModelType = viewModelType.WhenNotNull();
                _viewHandler = viewHandler.WhenNotNull();
                _ = viewHandler.WhenNotNull();

                OnChange += onChangeHandler;
            }

            public void AddView(IViewFor view, TViewId id)
            {
                _ = view.WhenNotNull();

                _ = GetView(view, false);

                var viewItem = new ViewItem<TViewId>
                {
                    View = view,
                    Id = id
                };

                _viewItems.Add(viewItem);

                var eventArgs = new ViewRegistryEventArgs(_viewModelType, view, ViewItemUpdateType.Add);

                OnChange.Invoke(this, eventArgs);
            }

            // returns true if there is at least one view remaining
            public bool RemoveView(IViewFor view)
            {
                _ = view.WhenNotNull();

                var viewItem = GetView(view, true);

                _ = _viewItems.Remove(viewItem);

                var eventArgs = new ViewRegistryEventArgs(_viewModelType, view, ViewItemUpdateType.Remove);

                OnChange.Invoke(this, eventArgs);

                return _viewItems.Count > 0;
            }

            public void ActivateAll()
            {
                foreach (var viewItem in _viewItems)
                {
                    _viewHandler.Activate(viewItem.View);
                }
            }

            private ViewItem<TViewId> GetView(IViewFor view, bool shouldExist)
            {
                var viewItem = _viewItems.SingleOrDefault(item => item.View == view);

                // TODO: Custom exception
                Throw<InvalidOperationException>.When(
                    !shouldExist && viewItem is not null,
                    "The view is already present in the view registry.");

                Throw<InvalidOperationException>.When(
                    shouldExist && viewItem is null,
                    "The view was not found in the view registry.");

                return viewItem!;
            }
        }

        private readonly Dictionary<Type, ViewRegistryItem> _viewRegistry = [];
        private readonly IViewFactory _viewFactory;
        private readonly IViewHandler _viewHandler;

        /// <inheritdoc />
        public event ViewRegistryEventHandler? OnUpdate;     // raised when a view is added or removed

        /// <inheritdoc />
        public bool IsEmpty => _viewRegistry.Keys.Count == 0;

        /// <summary>Constructor.</summary>
        /// <param name="viewFactory">The view factory used to create each new view.</param>
        /// <param name="viewHandler">The view handler used for activating, showing, and closing views. This handler
        /// will be platform specific.</param>
        public ViewRegistry(IViewFactory viewFactory, IViewHandler viewHandler)
        {
            _viewFactory = viewFactory.WhenNotNull();
            _viewHandler = viewHandler.WhenNotNull();
        }

        /// <inheritdoc />
        public int GetViewCountFor<TViewModel>() where TViewModel : class
        {
            return GetViewsFor<TViewModel>().Length;
        }

        /// <inheritdoc />
        public int GetViewCountFor(Type viewModelType)
        {
            return GetViewsFor(viewModelType).Length;
        }

        /// <inheritdoc />
        public Type[] GetViewModelTypes()
        {
            return [.. _viewRegistry.Keys];
        }

        /// <inheritdoc />
        public ViewItem<TViewId>[] GetViewsFor<TViewModel>() where TViewModel : class
        {
            var viewModelType = typeof(TViewModel);

            return GetViewsFor(viewModelType);
        }

        /// <inheritdoc />
        public ViewItem<TViewId>[] GetViewsFor(Type viewModelType)
        {
            if (!_viewRegistry.TryGetValue(viewModelType, out var registryItem))
            {
                return [];
            }

            return registryItem.Views;
        }

        /// <inheritdoc />
        public void CreateOrActivateFor<TViewModel>(int maxCount, Func<ViewItem<TViewId>[], TViewId> nextViewId,
            Action<TViewModel, IViewFor, TViewId>? configure = default) where TViewModel : class
        {
            _ = nextViewId.WhenNotNull();

            var viewModelType = typeof(TViewModel);

            // If the view model type already exists and the maximum number of views is already present then just restore them all.
            if (_viewRegistry.TryGetValue(viewModelType, out var registryItem) && registryItem.ViewCount >= maxCount)
            {
                // Activate all instances of this view type.
                registryItem.ActivateAll();

                return;
            }

            // Create a new instance of the view type.
            var view = _viewFactory.CreateViewFor<TViewModel>();
            var viewModel = view.ViewModel!;

            // If there were no instances of the view then create the initial cache item.
            if (registryItem is null)
            {
                registryItem = new ViewRegistryItem(viewModelType, _viewHandler, OnViewRegistryUpdate);

                _viewRegistry.Add(viewModelType, registryItem);
            }

            var viewId = nextViewId.Invoke(registryItem.Views);

            configure?.Invoke(viewModel, view, viewId);

            registryItem.AddView(view, viewId);

            _viewHandler.SetOnClosedHandler(view, OnViewClosedHandler, true);

            _viewHandler.Show(view);
        }

        /// <inheritdoc />
        public bool TryCloseAllViews()
        {
            // Need to get all views in advance as they cannot be closed during iteration (the collection will be modified)
            var views = this.SelectToReadOnlyCollection(item => item.View);

            foreach (var view in views)
            {
                _viewHandler.Close(view);
            }

            return IsEmpty;
        }

        /// <summary>Provides support for enumerating the registered views.</summary>
        /// <returns>An <see cref="IEnumerator"/> that allows for iteration of the registered views.</returns>
        public IEnumerator<ViewModelViewItem<TViewId>> GetEnumerator()
        {
            foreach (var kvp in _viewRegistry)
            {
                var viewModelType = kvp.Key;

                foreach (var view in kvp.Value.Views)
                {
                    yield return new ViewModelViewItem<TViewId>
                    {
                        ViewModelType = viewModelType,
                        View = view.View,
                        Id = view.Id
                    };
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void OnViewClosedHandler(object? sender, EventArgs eventArgs)
        {
            var view = (sender as IViewFor)!;

            _viewHandler.SetOnClosedHandler(view, OnViewClosedHandler, false);

            var viewModelType = view.GetType()
                .GetBaseGenericTypeDefinition(typeof(IViewFor<>))!
                .GenericTypeArguments[0];

            var registryItem = _viewRegistry[viewModelType];

            // Remove the window instance and if there's no more instances then remove the view type from the cache.
            if (!registryItem.RemoveView(view))
            {
                registryItem.OnChange -= OnViewRegistryUpdate;

                _viewRegistry.Remove(viewModelType);
            }
        }

        private void OnViewRegistryUpdate(object? sender, EventArgs eventArgs)
        {
            var viewRegistryEventArgs = eventArgs as ViewRegistryEventArgs;

            OnUpdate?.Invoke(sender, viewRegistryEventArgs!);
        }
    }
}