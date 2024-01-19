using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace AllOverIt.Reactive.Extensions
{
    /// <summary>Provides extension methods for property notification types, such as <see cref="INotifyPropertyChanging"/> and <see cref="INotifyPropertyChanged"/>.</summary>
    public static class PropertyNotifyExtensions
    {
        /// <summary>Gets an observable that pushes a notification each time a property value is about to change.</summary>
        /// <typeparam name="TType">The source type containing the required property.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="propertyExpression">The expression providing access to the property.</param>
        /// <returns>An observable that pushes a notification each time a property value is about to change.</returns>
        public static IObservable<TProperty> WhenPropertyChanging<TType, TProperty>(this TType source, Expression<Func<TType, TProperty>> propertyExpression)
            where TType : class, INotifyPropertyChanging
        {
            _ = source.WhenNotNull(nameof(source));

            var propertyName = propertyExpression
                .WhenNotNull(nameof(propertyExpression))
                .GetPropertyOrFieldMemberInfo().Name;

            return Observable
                .FromEventPattern<PropertyChangingEventHandler, PropertyChangingEventArgs>(
                    handler => source.PropertyChanging += handler,
                    handler => source.PropertyChanging -= handler)
                .Where(eventPattern => eventPattern.EventArgs.PropertyName == propertyName)
                .Select(_ => propertyExpression.Compile().Invoke(source));
        }

        /// <summary>Gets an observable that pushes a notification each time a property value has changed.</summary>
        /// <typeparam name="TType">The source type containing the required property.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="propertyExpression">The expression providing access to the property.</param>
        /// <returns>An observable that pushes a notification each time a property value has changed.</returns>
        public static IObservable<TProperty> WhenPropertyChanged<TType, TProperty>(this TType source, Expression<Func<TType, TProperty>> propertyExpression)
            where TType : class, INotifyPropertyChanged
        {
            _ = source.WhenNotNull(nameof(source));

            var propertyName = propertyExpression
                .WhenNotNull(nameof(propertyExpression))
                .GetPropertyOrFieldMemberInfo().Name;

            return Observable
                .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => source.PropertyChanged += handler,
                    handler => source.PropertyChanged -= handler)
                .Where(eventPattern => eventPattern.EventArgs.PropertyName == propertyName)
                .Select(_ => propertyExpression.Compile().Invoke(source));
        }
    }
}
