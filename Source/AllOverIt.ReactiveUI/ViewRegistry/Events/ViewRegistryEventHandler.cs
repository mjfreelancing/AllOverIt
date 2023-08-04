﻿using ReactiveUI;
using System;

namespace AllOverIt.ReactiveUI.ViewRegistry.Events
{
    /// <summary>Event arguments provided to a <see cref="ViewRegistryEventHandler"/>.</summary>
    public sealed class ViewRegistryEventArgs : EventArgs
    {
        /// <summary>The view model type associated with the <see cref="View"/>.</summary>
        public Type ViewModelType { get; }

        /// <summary>The view associated with the raised event.</summary>
        public IViewFor View { get; }

        /// <summary>Constructor.</summary>
        /// <param name="viewModelType">The view model type associated with the <see cref="View"/>.</param>
        /// <param name="view">The view associated with the raised event.</param>
        public ViewRegistryEventArgs(Type viewModelType, IViewFor view)
        {
            ViewModelType = viewModelType;
            View = view;
        }
    }

    /// <summary>Describes an event handler raised by an <see cref="IViewRegistry{TViewId}"/>.</summary>
    /// <param name="sender">The sender raising the event.</param>
    /// <param name="eventArgs">The arguments for the event.</param>
    public delegate void ViewRegistryEventHandler(object sender, ViewRegistryEventArgs eventArgs);
}