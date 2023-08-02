using ReactiveUI;
using System;

namespace AllOverIt.ReactiveUI.ViewRegistry.Events
{
    public sealed class ViewRegistryEventArgs : EventArgs
    {
        public Type ViewModelType { get; }
        public IViewFor View { get; }

        public ViewRegistryEventArgs(Type viewModelType, IViewFor view)
        {
            ViewModelType = viewModelType;
            View = view;
        }
    }

    public delegate void ViewRegistryEventHandler(object sender, ViewRegistryEventArgs eventArgs);
}