using System;
using System.Collections.Generic;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    public interface IViewRegistry<TViewId>
    {
        event EventHandler OnUpdate;

        bool IsEmpty { get; }

        int GetViewCountFor<TViewModel>() where TViewModel : class;

        void CreateOrActivateFor<TViewModel>(int maxCount, Func<IReadOnlyCollection<ViewItem<TViewId>>, TViewId> nextViewId,
            Action<TViewModel> configure = default) where TViewModel : class;
    }
}