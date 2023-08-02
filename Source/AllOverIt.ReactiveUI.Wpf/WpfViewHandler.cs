using AllOverIt.Assertion;
using AllOverIt.ReactiveUI.ViewRegistry;
using ReactiveUI;
using System;
using System.Windows;

namespace AllOverIt.ReactiveUI.Wpf
{
    public sealed class WpfViewHandler : IViewHandler
    {
        public void Activate(IViewFor view)
        {
            GetWindow(view).Activate();
        }

        public void Show(IViewFor view)
        {
            GetWindow(view).Show();
        }

        public void Close(IViewFor view)
        {
            GetWindow(view).Close();
        }

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
