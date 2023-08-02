using ReactiveUI;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    public sealed class ViewItem<TViewId>
    {
        public IViewFor View { get; init; }
        public TViewId Id { get; init; }
    }
}