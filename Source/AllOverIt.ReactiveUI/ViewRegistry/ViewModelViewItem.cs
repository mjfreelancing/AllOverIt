using ReactiveUI;
using System;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    public sealed class ViewModelViewItem<TViewId>
    {
        public Type ViewModelType { get; init; }
        public IViewFor View { get; init; }
        public TViewId Id { get; init; }
    }
}