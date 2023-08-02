using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.ReactiveUI.Factories;
using AllOverIt.ReactiveUI.ViewRegistry.Events;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    public abstract class ViewRegistry<TViewId> : IViewRegistry<TViewId>
    {
        private class ViewRegistryItem
        {
            private readonly Type _viewModelType;
            private readonly IViewHandler _viewHandler;
            private readonly IList<ViewItem<TViewId>> _viewItems = new List<ViewItem<TViewId>>();

            public event ViewRegistryEventHandler OnChange;     // raised when a view is added or removed

            public IReadOnlyCollection<ViewItem<TViewId>> Views => _viewItems.AsReadOnlyCollection();
            public int ViewCount => _viewItems.Count;

            public ViewRegistryItem(Type viewModelType, IViewHandler viewHandler)
            {
                _viewModelType = viewModelType.WhenNotNull(nameof(viewModelType));
                _viewHandler = viewHandler.WhenNotNull(nameof(viewHandler));
            }

            public void AddView(IViewFor view, TViewId id)
            {
                _ = view.WhenNotNull(nameof(view));

                _ = GetView(view, false);
                
                var viewItem = new ViewItem<TViewId>
                {
                    View = view,
                    Id = id
                };

                _viewItems.Add(viewItem);

                var eventArgs = new ViewRegistryEventArgs(_viewModelType, view);

                OnChange?.Invoke(this, eventArgs);
            }

            // returns true if there is at least one view remaining
            public bool RemoveView(IViewFor view)
            {
                _ = view.WhenNotNull(nameof(view));

                var viewItem = GetView(view, true);

                _ = _viewItems.Remove(viewItem);

                var eventArgs = new ViewRegistryEventArgs(_viewModelType, view);

                OnChange?.Invoke(this, eventArgs);

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

                return viewItem;
            }
        }

        private readonly IDictionary<Type, ViewRegistryItem> _viewRegistry = new Dictionary<Type, ViewRegistryItem>();
        private readonly IViewFactory _viewFactory;
        private readonly IViewHandler _viewHandler;

        public event ViewRegistryEventHandler OnUpdate;     // raised when a view is added or removed

        public bool IsEmpty => !_viewRegistry.Any();

        protected ViewRegistry(IViewFactory viewFactory, IViewHandler viewHandler)
        {
            _viewFactory = viewFactory.WhenNotNull(nameof(viewFactory));
            _viewHandler = viewHandler.WhenNotNull(nameof(viewHandler));
        }

        public int GetViewCountFor<TViewModel>() where TViewModel : class
        {
            return GetViewsFor<TViewModel>().Count;
        }

        public int GetViewCountFor(Type viewModelType)
        {
            return GetViewsFor(viewModelType).Count;
        }

        public IReadOnlyCollection<Type> GetViewModelTypes()
        {
            return _viewRegistry.Keys.AsReadOnlyCollection();
        }

        public IReadOnlyCollection<ViewItem<TViewId>> GetViewsFor<TViewModel>() where TViewModel : class
        {
            var viewModelType = typeof(TViewModel);

            return GetViewsFor(viewModelType);
        }

        public IReadOnlyCollection<ViewItem<TViewId>> GetViewsFor(Type viewModelType)
        {
            if (!_viewRegistry.TryGetValue(viewModelType, out var registryItem))
            {
                return Collections.Collection.EmptyReadOnly<ViewItem<TViewId>>();
            }

            return registryItem.Views;
        }

        public void CreateOrActivateFor<TViewModel>(int maxCount, Func<IReadOnlyCollection<ViewItem<TViewId>>, TViewId> nextViewId,
            Action<TViewModel, TViewId> configure = default) where TViewModel : class
        {
            _ = nextViewId.WhenNotNull(nameof(nextViewId));

            var viewModelType = typeof(TViewModel);

            // If the view model type already exists and the maximum number of views is already present then just restore them all.
            if (_viewRegistry.TryGetValue(viewModelType, out var registryItem) && registryItem.ViewCount >= maxCount)
            {
                // Activate all instances of this view type.
                registryItem.ActivateAll();

                return;
            }

            // Create a new instance of the view typw.
            var view = _viewFactory.CreateViewFor<TViewModel>();
            var viewModel = view.ViewModel;

            // If there were no instances of the view then create the initial cache item.
            if (registryItem is null)
            {
                registryItem = new ViewRegistryItem(viewModelType, _viewHandler);

                registryItem.OnChange += OnCacheItemUpdate;

                _viewRegistry.Add(viewModelType, registryItem);
            }

            var viewId = nextViewId.Invoke(registryItem.Views);

            configure?.Invoke(viewModel, viewId);

            registryItem.AddView(view, viewId);

            _viewHandler.SetOnClosedHandler(view, OnViewClosedHandler, true);

            _viewHandler.Show(view);
        }

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

        private void OnViewClosedHandler(object sender, EventArgs eventArgs)
        {
            var view = sender as IViewFor;

            _viewHandler.SetOnClosedHandler(view, OnViewClosedHandler, false);

            var viewModelType = view.GetType().BaseType.GenericTypeArguments[0];

            if (!_viewRegistry.TryGetValue(viewModelType, out var registryItem))
            {
                throw new InvalidOperationException("Did not find the required view in the registry.");
            }

            // Remove the window instance and if there's no more instances then remove the view type from the cache.
            if (!registryItem.RemoveView(view))
            {
                registryItem.OnChange -= OnCacheItemUpdate;

                _viewRegistry.Remove(viewModelType);
            }
        }

        private void OnCacheItemUpdate(object sender, EventArgs eventArgs)
        {
            OnUpdate?.Invoke(sender, eventArgs as ViewRegistryEventArgs);
        }
    }
}