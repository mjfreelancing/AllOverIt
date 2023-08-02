﻿using ReactiveUI;
using System;

namespace AllOverIt.ReactiveUI.ViewRegistry
{
    // Platform specific, such as (view as Window).Activate()
    public interface IViewHandler
    {
        void Activate(IViewFor view);
        void Show(IViewFor view);
        void Close(IViewFor view);

        // 'register' indicates whether the event handler is subscribing or unsubscribing
        void SetOnActivatedHandler(IViewFor view, EventHandler eventHandler, bool register);
        void SetOnDeactivatedHandler(IViewFor view, EventHandler eventHandler, bool register);
        void SetOnClosedHandler(IViewFor view, EventHandler eventHandler, bool register);
    }
}