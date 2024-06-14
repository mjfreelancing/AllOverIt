#nullable enable

using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using FluentAssertions;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Fixture.Assertions
{
    // Don't move ClassPropertiesExtensions - less troublesome to use if it has the same namespace as Properties.For<T>()

    /// <summary>Provides extension methods for the <see cref="ClassPropertiesAssertions{TType}"/>.</summary>
    public static class ClassPropertiesExtensions
    {
        /// <summary>Gets a <see cref="ClassPropertiesAssertions{TType}"/> that can be used to assert properties on the specified type.</summary>
        /// <typeparam name="TType">The type to assert its' property definitions.</typeparam>
        /// <param name="instance">The <see cref="ClassProperties{TType}"/> containing the property info to assert.</param>
        /// <returns>A <see cref="ClassPropertiesAssertions{TType}"/> that can be used to assert properties on the specified type.</returns>
        public static ClassPropertiesAssertions<TType> Should<TType>(this ClassProperties<TType> instance)
        {
            return new ClassPropertiesAssertions<TType>(instance);
        }
    }

    /// <summary>Provides methods for filtering class properties that can be asserted.</summary>
    /// <typeparam name="TType">The type to assert its' property definitions.</typeparam>
    public sealed class ClassProperties<TType>
    {
        private IEnumerable<PropertyInfo>? _properties;

        private readonly PropertyInfo[] _allProperties;

        /// <summary>Gets the currently filtered property names.</summary>
        internal PropertyInfo[] Properties => [.. (_properties ?? _allProperties)];

        /// <summary>Constructor.</summary>
        /// <param name="declaredOnly">When <see langword="True"/>, base class properties will be ignored.</param>
        public ClassProperties(bool declaredOnly)
        {
            _allProperties = typeof(TType).GetPropertyInfo(BindingOptions.All, declaredOnly).ToArray();
        }

        /// <summary>Filters the properties to only those specified in <paramref name="propertyNames"/>. Property names
        /// that do not exist are ignored. Multiple calls to this method will further filter the results.</summary>
        /// <param name="propertyNames">The property names to be included.</param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        public ClassProperties<TType> Including(params string[] propertyNames)
        {
            _ = propertyNames.WhenNotNullOrEmpty(nameof(propertyNames));

            _properties = (_properties ?? _allProperties).Where(propInfo => propertyNames.Contains(propInfo.Name));

            return this;
        }

        /// <summary>Filters the properties to only those specified in the provided property expressions. Property names
        /// that do not exist are ignored. Multiple calls to this method will further filter the results.</summary>
        /// <param name="properties">Onr or more property expressions.</param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        public ClassProperties<TType> Including(params Expression<Func<TType, object>>[] properties)
        {
            _ = properties.WhenNotNullOrEmpty(nameof(properties));

            var propertyNames = properties.SelectToArray(property => property.UnwrapMemberExpression().Member.Name);

            return Including(propertyNames);
        }

        /// <summary>Filters the properties by excluding those specified in <paramref name="propertyNames"/>. Multiple calls
        /// to this method will further filter the results.</summary>
        /// <param name="propertyNames">The property names to be excluded.</param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        public ClassProperties<TType> Excluding(params string[] propertyNames)
        {
            _ = propertyNames.WhenNotNullOrEmpty(nameof(propertyNames));

            _properties = (_properties ?? _allProperties).Where(propInfo => !propertyNames.Contains(propInfo.Name));

            return this;
        }

        /// <summary>Filters the properties by excluding those specified in <paramref name="properties"/>. Multiple calls
        /// to this method will further filter the results.</summary>
        /// <param name="propertyNames">The property names to be excluded.</param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        public ClassProperties<TType> Excluding(params Expression<Func<TType, object>>[] properties)
        {
            _ = properties.WhenNotNullOrEmpty(nameof(properties));

            var propertyNames = properties.SelectToArray(property => property.UnwrapMemberExpression().Member.Name);

            return Excluding(propertyNames);
        }

        /// <summary>Filters the current collection of properties based on the specified <paramref name="predicate"/>.
        /// If <see cref="Including(string[])"/> or <see cref="Excluding(string[])"/> has not been called, then the
        /// predicate is applied against all properties on <typeparamref name="TType"/>.</summary>
        /// <param name="predicate">The predicate to filter the properties.</param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        public ClassProperties<TType> Where(Func<PropertyInfo, bool> predicate)
        {
            _ = predicate.WhenNotNull(nameof(predicate));

            _properties = (_properties ?? _allProperties).Where(predicate);

            return this;
        }
    }
}
