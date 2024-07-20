using AllOverIt.Assertion;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AllOverIt.Reactive
{
    /// <summary>An abstract observable class implementing <see cref="INotifyPropertyChanged"/> and <see cref="INotifyPropertyChanging"/>.</summary>
    public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <inheritdoc cref="INotifyPropertyChanging.PropertyChanging"/>
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Compares the current and new value of a property. If the new value is different then the <see cref="PropertyChanging"/> event
        /// is called, then the value is updated, then the <see cref="PropertyChanged"/> event is called.</summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="backingField">The backing field storing the property value.</param>
        /// <param name="newValue">The new property value to be set.</param>
        /// <param name="propertyName">The name of the property that is changing. Optional</param>
        /// <returns><see langword="True"/> if the property value was changed, otherwise <see langword="False"/>.</returns>
        protected bool RaiseAndSetIfChanged<TProperty>(ref TProperty? backingField, TProperty? newValue, [CallerMemberName] string propertyName = "")
        {
            return RaiseAndSetIfChanged(ref backingField, newValue, null, null, null, propertyName);
        }

        /// <summary>Compares the current and new value of a property. If the new value is different then the <see cref="PropertyChanging"/> event
        /// is called, then the value is updated, then the <see cref="PropertyChanged"/> event is called.</summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="backingField">The backing field storing the property value.</param>
        /// <param name="newValue">The new property value to be set.</param>
        /// <param name="onChanging">An action to be invoked before the property value is changed. Optional.</param>
        /// <param name="onChanged">An action to be invoked after the property value is changed. Optional.</param>
        /// <param name="propertyName">The name of the property that is changing. Optional</param>
        /// <returns><see langword="True"/> if the property value was changed, otherwise <see langword="False"/>.</returns>
        protected bool RaiseAndSetIfChanged<TProperty>(ref TProperty? backingField, TProperty? newValue, Action? onChanging, Action? onChanged,
            [CallerMemberName] string propertyName = "")
        {
            return RaiseAndSetIfChanged(ref backingField, newValue, null, onChanging, onChanged, propertyName);
        }

        /// <summary>Compares the current and new value of a property. If the new value is different then the <see cref="PropertyChanging"/> event
        /// is called, then the value is updated, then the <see cref="PropertyChanged"/> event is called.</summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="backingField">The backing field storing the property value.</param>
        /// <param name="newValue">The new property value to be set.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{TProperty}"/> instance to use to compare the old and new property values.</param>
        /// <param name="onChanging">An action to be invoked before the property value is changed. Optional.</param>
        /// <param name="onChanged">An action to be invoked after the property value is changed. Optional.</param>
        /// <param name="propertyName">The name of the property that is changing. Optional</param>
        /// <returns><see langword="True"/> if the property value was changed, otherwise <see langword="False"/>.</returns>
        protected bool RaiseAndSetIfChanged<TProperty>(ref TProperty backingField, TProperty newValue, IEqualityComparer<TProperty>? comparer,
            Action? onChanging, Action? onChanged, [CallerMemberName] string propertyName = "")
        {
            comparer ??= EqualityComparer<TProperty>.Default;

            if (comparer.Equals(backingField, newValue))
            {
                return false;
            }

            RaisePropertyChanging(propertyName);
            onChanging?.Invoke();

            backingField = newValue;

            RaisePropertyChanged(propertyName);
            onChanged?.Invoke();

            return true;
        }

        /// <summary>Raises the <see cref="PropertyChanging"/> event.</summary>
        /// <param name="propertyName">The property name changing.</param>
        protected void RaisePropertyChanging([CallerMemberName] string propertyName = "")
        {
            RaisePropertyChanging(new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>Raises the <see cref="PropertyChanged"/> event.</summary>
        /// <param name="propertyName">The property name changed.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void RaisePropertyChanging(PropertyChangingEventArgs eventArgs)
        {
            _ = eventArgs.WhenNotNull();

            PropertyChanging?.Invoke(this, eventArgs);
        }

        private void RaisePropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            _ = eventArgs.WhenNotNull();

            PropertyChanged?.Invoke(this, eventArgs);
        }
    }
}
