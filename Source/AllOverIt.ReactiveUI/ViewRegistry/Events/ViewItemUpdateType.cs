namespace AllOverIt.ReactiveUI.ViewRegistry.Events
{
    /// <summary>The update type for a <see cref="ViewItem{TViewId}"/> managed by a <see cref="ViewRegistry{TViewId}"/>.</summary>
    public enum ViewItemUpdateType
    {
        /// <summary>The <see cref="ViewItem{TViewId}"/> was added to the <see cref="ViewRegistry{TViewId}"/>.</summary>
        Add,

        /// <summary>The <see cref="ViewItem{TViewId}"/> is being removed from the <see cref="ViewRegistry{TViewId}"/>.</summary>
        Remove
    }
}