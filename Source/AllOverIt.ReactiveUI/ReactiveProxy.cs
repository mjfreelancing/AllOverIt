﻿using AllOverIt.Assertion;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AllOverIt.ReactiveUI
{
    /// <summary>Extends <see cref="ReactiveObject"/> by adding support for creating a wrapper around a model that is not
    /// observable. This can be useful for sitations such as creating an observable model for use in a MVVM application
    /// that wraps a non-observable entity. Property updates made via this proxy object will apply the same updates to the 
    /// wrapped model and raise change notifications.</summary>
    /// <typeparam name="TType">The wrapped model type.</typeparam>
    public abstract class ReactiveProxy<TType> : ReactiveObject where TType : class
    {
        /// <summary>The wrapped model instance.</summary>
        public readonly TType Model;

        /// <summary>Constructor.</summary>
        /// <param name="model">The wrapped model instance.</param>
        protected ReactiveProxy(TType model)
        {
            Model = model.WhenNotNull(nameof(model));
        }


        /// <summary>Compares the current and new value of a property on the wrapped <see cref="Model"/>. If the new value is
        /// different then the <see cref="ReactiveObject.PropertyChanging"/> event is called, then the value is updated via <paramref name="setValue"/>,
        /// then the <see cref="ReactiveObject.PropertyChanged"/> event is called.</summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The new property value to be set.</param>
        /// <param name="setValue">The action to update the value on the wrapped <see cref="Model"/>.</param>
        /// <param name="propertyName">The name of the property that is changing. Optional</param>
        /// <returns><see langword="True"/> if the property value was changed, otherwise <see langword="false"/>.</returns>
        protected bool RaiseAndSetIfChanged<TProperty>(TProperty oldValue, TProperty newValue, Action<TType, TProperty> setValue,
            [CallerMemberName] string propertyName = "")
        {
            return RaiseAndSetIfChanged(oldValue, newValue, setValue, null, null, null, propertyName);
        }

        /// <summary>Compares the current and new value of a property on the wrapped <see cref="Model"/>. If the new value is
        /// different then the <see cref="ReactiveObject.PropertyChanging"/> event is called, then the value is updated via <paramref name="setValue"/>,
        /// then the <see cref="ReactiveObject.PropertyChanged"/> event is called.</summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The new property value to be set.</param>
        /// <param name="setValue">The action to update the value on the wrapped <see cref="Model"/>.</param>
        /// <param name="onChanging">An action to be invoked before the property value is changed. Optional.</param>
        /// <param name="onChanged">An action to be invoked after the property value is changed. Optional.</param>
        /// <param name="propertyName">The name of the property that is changing. Optional</param>
        /// <returns><see langword="True"/> if the property value was changed, otherwise <see langword="false"/>.</returns>
        protected bool RaiseAndSetIfChanged<TProperty>(TProperty oldValue, TProperty newValue, Action<TType, TProperty> setValue,
            Action onChanging, Action onChanged, [CallerMemberName] string propertyName = "")
        {
            return RaiseAndSetIfChanged(oldValue, newValue, setValue, null, onChanging, onChanged, propertyName);
        }

        /// <summary>Compares the current and new value of a property on the wrapped <see cref="Model"/>. If the new value is
        /// different then the <see cref="ReactiveObject.PropertyChanging"/> event is called, then the value is updated via <paramref name="setValue"/>,
        /// then the <see cref="ReactiveObject.PropertyChanged"/> event is called.</summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The new property value to be set.</param>
        /// <param name="setValue">The action to update the value on the wrapped <see cref="Model"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{TProperty}"/> instance to use to compare the old and new property values.</param>
        /// <param name="onChanging">An action to be invoked before the property value is changed. Optional.</param>
        /// <param name="onChanged">An action to be invoked after the property value is changed. Optional.</param>
        /// <param name="propertyName">The name of the property that is changing. Optional</param>
        /// <returns><see langword="True"/> if the property value was changed, otherwise <see langword="false"/>.</returns>
        protected bool RaiseAndSetIfChanged<TProperty>(TProperty oldValue, TProperty newValue, Action<TType, TProperty> setValue,
            IEqualityComparer<TProperty> comparer, Action onChanging, Action onChanged, [CallerMemberName] string propertyName = "")
        {
            _ = setValue.WhenNotNull(nameof(setValue));

            comparer ??= EqualityComparer<TProperty>.Default;

            if (comparer.Equals(oldValue, newValue))
            {
                return false;
            }

            this.RaisePropertyChanging(propertyName);
            onChanging?.Invoke();

            setValue.Invoke(Model, newValue);

            this.RaisePropertyChanged(propertyName);
            onChanged?.Invoke();

            return true;
        }
    }
}