using ReactiveUI;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    /// <summary>Holds a reference to a view registered with an <see cref="IViewRegistry{TViewId}"/> and its' associated Id.</summary>
    /// <typeparam name="TViewId">The type of the view's Id.</typeparam>
    public sealed class ViewItem<TViewId>
    {
        /// <summary>The registered view.</summary>
        public IViewFor View { get; init; }

        /// <summary>The view's Id.</summary>
        public TViewId Id { get; init; }
    }
}