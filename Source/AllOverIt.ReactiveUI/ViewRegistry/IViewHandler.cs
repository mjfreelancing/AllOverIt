using ReactiveUI;
using System;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    // Platform specific, such as (view as Window).Activate()
    public interface IViewHandler
    {
        void Activate(IViewFor view);
        void Show(IViewFor view);
        void SetOnActivatedHandler(IViewFor view, EventHandler eventHandler);           // pass null to unregister
        void SetOnDeactivatedHandler(IViewFor view, EventHandler eventHandler);         // pass null to unregister
        void SetOnClosedHandler(IViewFor view, EventHandler eventHandler);              // pass null to unregister
    }
}