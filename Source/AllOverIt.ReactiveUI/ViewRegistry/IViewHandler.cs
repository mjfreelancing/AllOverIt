using ReactiveUI;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    /// <summary>Provides an abstraction over the handling of view (typically window) operations such as activating,
    /// showing, and closing them.</summary>
    public interface IViewHandler
    {
        /// <summary>Activates the specified view to bring it to the foreground.</summary>
        /// <param name="view">The view to activate.</param>
        void Activate(IViewFor view);

        /// <summary>Shows the specified view.</summary>
        /// <param name="view">The view to show.</param>
        void Show(IViewFor view);

        /// <summary>Closes the specified view.</summary>
        /// <param name="view">The view to close.</param>
        void Close(IViewFor view);

        /// <summary>Register or unregister an OnActivated event handler for a specified view.</summary>
        /// <param name="view">The view to registers or unregister an OnActivated event handler.</param>
        /// <param name="eventHandler">The OnActivated event handler.</param>
        /// <param name="register"><see langword="True"/> to register the handler, otherwise unregister the handler.</param>
        void SetOnActivatedHandler(IViewFor view, EventHandler eventHandler, bool register);

        /// <summary>Register or unregister an OnDeactivated event handler for a specified view.</summary>
        /// <param name="view">The view to registers or unregister an OnDeactivated event handler.</param>
        /// <param name="eventHandler">The OnDeactivated event handler.</param>
        /// <param name="register"><see langword="True"/> to register the handler, otherwise unregister the handler.</param>
        void SetOnDeactivatedHandler(IViewFor view, EventHandler eventHandler, bool register);

        /// <summary>Register or unregister an OnClosed event handler for a specified view.</summary>
        /// <param name="view">The view to registers or unregister an OnClosed event handler.</param>
        /// <param name="eventHandler">The OnClosed event handler.</param>
        /// <param name="register"><see langword="True"/> to register the handler, otherwise unregister the handler.</param>
        void SetOnClosedHandler(IViewFor view, EventHandler eventHandler, bool register);
    }
}