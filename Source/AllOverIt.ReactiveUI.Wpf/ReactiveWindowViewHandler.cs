using AllOverIt.Assertion;
using AllOverIt.ReactiveUI.ViewRegistry;
using ReactiveUI;
using System;
using System.Windows;

namespace AllOverIt.ReactiveUI.Wpf
{
    /// <summary>Implements a view handler for WPF <see cref="ReactiveWindow{TViewModel}"/> instances.</summary>
    public sealed class ReactiveWindowViewHandler : IViewHandler
    {
        /// <inheritdoc />
        /// <remarks>The view is expected to be a <see cref="ReactiveWindow{TViewModel}"/>.</remarks>
        public void Activate(IViewFor view)
        {
            GetWindow(view).Activate();
        }

        /// <inheritdoc />
        /// <remarks>The view is expected to be a <see cref="ReactiveWindow{TViewModel}"/>.</remarks>
        public void Show(IViewFor view)
        {
            GetWindow(view).Show();
        }

        /// <inheritdoc />
        /// <remarks>The view is expected to be a <see cref="ReactiveWindow{TViewModel}"/>.</remarks>
        public void Close(IViewFor view)
        {
            GetWindow(view).Close();
        }

        /// <inheritdoc />
        /// <remarks>The view is expected to be a <see cref="ReactiveWindow{TViewModel}"/>.</remarks>
        public void SetOnActivatedHandler(IViewFor view, EventHandler eventHandler, bool register)
        {
            _ = eventHandler.WhenNotNull();

            if (register)
            {
                GetWindow(view).Activated += eventHandler;
            }
            else
            {
                GetWindow(view).Activated -= eventHandler;
            }
        }

        /// <inheritdoc />
        /// <remarks>The view is expected to be a <see cref="ReactiveWindow{TViewModel}"/>.</remarks>
        public void SetOnDeactivatedHandler(IViewFor view, EventHandler eventHandler, bool register)
        {
            _ = eventHandler.WhenNotNull();

            if (register)
            {
                GetWindow(view).Deactivated += eventHandler;
            }
            else
            {
                GetWindow(view).Deactivated -= eventHandler;
            }
        }

        /// <inheritdoc />
        /// <remarks>The view is expected to be a <see cref="ReactiveWindow{TViewModel}"/>.</remarks>
        public void SetOnClosedHandler(IViewFor view, EventHandler eventHandler, bool register)
        {
            _ = eventHandler.WhenNotNull();

            if (register)
            {
                GetWindow(view).Closed += eventHandler;
            }
            else
            {
                GetWindow(view).Closed -= eventHandler;
            }
        }

        private static Window GetWindow(IViewFor view)
        {
            return view as Window;
        }
    }
}
