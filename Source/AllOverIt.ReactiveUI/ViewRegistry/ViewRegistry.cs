using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.ReactiveUI.Factories;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    public abstract class ViewRegistry<TViewId> : IViewRegistry<TViewId>
    {
        private class WindowRegistryItem
        {
            private readonly IViewHandler _viewHandler;
            private readonly IList<ViewItem<TViewId>> _viewItems = new List<ViewItem<TViewId>>();

            public event EventHandler OnChange;     // raised when a view is added or removed

            public IReadOnlyCollection<ViewItem<TViewId>> Views => _viewItems.AsReadOnlyCollection();
            public int ViewCount => _viewItems.Count;

            public WindowRegistryItem(IViewHandler viewHandler)
            {
                _viewHandler = viewHandler.WhenNotNull(nameof(viewHandler));
            }

            public void AddView(IViewFor view, TViewId id)
            {
                _ = view.WhenNotNull(nameof(view));

                var viewItem = GetView(view);
                
                // TODO: Custom exception
                Throw<InvalidOperationException>.WhenNotNull(viewItem, "The view is already present in the view registry.");

                var vieItem = new ViewItem<TViewId>
                {
                    View = view,
                    Id = id
                };

                _viewItems.Add(vieItem);

                OnChange?.Invoke(this, EventArgs.Empty);
            }

            // returns true if there is at least one view remaining
            public bool RemoveView(IViewFor view)
            {
                _ = view.WhenNotNull(nameof(view));

                var viewItem = GetView(view);

                _ = _viewItems.Remove(viewItem);

                OnChange?.Invoke(this, EventArgs.Empty);

                return _viewItems.Count > 0;
            }

            public void ActivateAll()
            {
                foreach (var viewItem in _viewItems)
                {
                    _viewHandler.Activate(viewItem.View);
                }
            }

            private ViewItem<TViewId> GetView(IViewFor view)
            {
                var viewItem = _viewItems.SingleOrDefault(item => item.View == view);

                // TODO: Custom exception
                Throw<InvalidOperationException>.WhenNull(viewItem, "The view was not found in the view registry.");

                return viewItem;
            }
        }

        private readonly IDictionary<Type, WindowRegistryItem> _viewCache = new Dictionary<Type, WindowRegistryItem>();
        private readonly IViewFactory _viewFactory;
        private readonly IViewHandler _viewHandler;

        public event EventHandler OnUpdate;     // raised when a view is added or removed

        public bool IsEmpty => !_viewCache.Any();

        protected ViewRegistry(IViewFactory viewFactory, IViewHandler viewHandler)
        {
            _viewFactory = viewFactory.WhenNotNull(nameof(viewFactory));
            _viewHandler = viewHandler.WhenNotNull(nameof(viewHandler));
        }

        public int GetViewCountFor<TViewModel>() where TViewModel : class
        {
            var viewModelType = typeof(TViewModel);

            if (!_viewCache.TryGetValue(viewModelType, out var cacheItem))
            {
                return 0;
            }

            return cacheItem.ViewCount;
        }

        public void CreateOrActivateFor<TViewModel>(int maxCount, Func<IReadOnlyCollection<ViewItem<TViewId>>, TViewId> nextViewId,
            Action<TViewModel> configure = default) where TViewModel : class
        {
            _ = nextViewId.WhenNotNull(nameof(nextViewId));

            var viewModelType = typeof(TViewModel);

            // If the view model type already exists and the maximum number of views is already present then just restore them all.
            if (_viewCache.TryGetValue(viewModelType, out var cacheItem) && cacheItem.ViewCount >= maxCount)
            {
                // Activate all instances of this view type.
                cacheItem.ActivateAll();

                return;
            }

            // Create a new instance of the view typw.
            var view = _viewFactory.CreateViewFor<TViewModel>();
            var viewModel = view.ViewModel;

            configure?.Invoke(viewModel);

            // If there were no instances of the view then create the initial cache item.
            if (cacheItem is null)
            {
                cacheItem = new WindowRegistryItem(_viewHandler);

                cacheItem.OnChange += OnCacheItemUpdate;

                _viewCache.Add(viewModelType, cacheItem);
            }

            var viewId = nextViewId.Invoke(cacheItem.Views);

            cacheItem.AddView(view, viewId);

            void OnViewClosedHandler(object sender, EventArgs eventArgs)
            {
                // Remove the window instance and if there's no more instances then remove the view type from the cache.
                if (!cacheItem.RemoveView(view))
                {
                    cacheItem.OnChange -= OnCacheItemUpdate;

                    _viewCache.Remove(viewModelType);
                }
            }

            _viewHandler.SetOnClosedHandler(view, OnViewClosedHandler);

            _viewHandler.Show(view);
        }

        private void OnCacheItemUpdate(object sender, EventArgs eventArgs)
        {
            OnUpdate?.Invoke(sender, eventArgs);
        }
    }
}