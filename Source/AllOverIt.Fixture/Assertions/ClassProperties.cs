#nullable enable

using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Fixture.Assertions
{
    /// <summary>Provides methods for filtering class properties that can be asserted.</summary>
    /// <typeparam name="TType">The type to assert its' property definitions.</typeparam>
    public sealed class ClassProperties<TType> : ClassPropertiesBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="declaredOnly">When <see langword="True"/>, base class properties will be ignored.</param>
        public ClassProperties(bool declaredOnly)
            : base(typeof(TType), declaredOnly)
        {
        }

        /// <summary>Filters the properties to only those specified in <paramref name="propertyNames"/>. Property names
        /// that do not exist are ignored. Multiple calls to this method will further filter the results.</summary>
        /// <param name="propertyNames">The property names to be included.</param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        public ClassProperties<TType> Including(params string[] propertyNames)
        {
            _ = propertyNames.WhenNotNullOrEmpty(nameof(propertyNames));

            IncludeProperties(propertyNames);

            return this;
        }

        /// <summary>Filters the properties to only those specified in the provided property expressions. Property names
        /// that do not exist are ignored. Multiple calls to this method will further filter the results.</summary>
        /// <param name="properties">One or more property expressions to include.</param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        public ClassProperties<TType> Including(params Expression<Func<TType, object>>[] properties)
        {
            _ = properties.WhenNotNullOrEmpty(nameof(properties));

            var propertyNames = properties.SelectToArray(property => property.UnwrapMemberExpression()!.Member.Name);

            return Including(propertyNames);
        }

        /// <summary>Filters the properties by excluding those specified in <paramref name="propertyNames"/>. Multiple calls
        /// to this method will further filter the results.</summary>
        /// <param name="propertyNames">The property names to be excluded.</param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        public ClassProperties<TType> Excluding(params string[] propertyNames)
        {
            _ = propertyNames.WhenNotNullOrEmpty(nameof(propertyNames));

            ExcludeProperties(propertyNames);

            return this;
        }

        /// <summary>Filters the properties by excluding those specified in <paramref name="properties"/>. Multiple calls
        /// to this method will further filter the results.</summary>
        /// <param name="properties">One or more property expressions to exclude.</param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        public ClassProperties<TType> Excluding(params Expression<Func<TType, object>>[] properties)
        {
            _ = properties.WhenNotNullOrEmpty(nameof(properties));

            var propertyNames = properties.SelectToArray(property => property.UnwrapMemberExpression()!.Member.Name);

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

            AddFilter(predicate);

            return this;
        }
    }

    /// <summary>Provides methods for filtering class properties that can be asserted.</summary>
    public sealed class ClassProperties : ClassPropertiesBase
    {
        /// <summary>Constructor.</summary>
        /// <param name="classType">The class type to assert its' property definitions.</param>
        /// <param name="declaredOnly">When <see langword="True"/>, base class properties will be ignored.</param>
        public ClassProperties(Type classType, bool declaredOnly)
            : base(classType, declaredOnly)
        {
        }

        /// <summary>Filters the properties to only those specified in <paramref name="propertyNames"/>. Property names
        /// that do not exist are ignored. Multiple calls to this method will further filter the results.</summary>
        /// <param name="propertyNames">The property names to be included.</param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        public ClassProperties Including(params string[] propertyNames)
        {
            _ = propertyNames.WhenNotNullOrEmpty(nameof(propertyNames));

            IncludeProperties(propertyNames);

            return this;
        }

        /// <summary>Filters the properties by excluding those specified in <paramref name="propertyNames"/>. Multiple calls
        /// to this method will further filter the results.</summary>
        /// <param name="propertyNames">The property names to be excluded.</param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        public ClassProperties Excluding(params string[] propertyNames)
        {
            _ = propertyNames.WhenNotNullOrEmpty(nameof(propertyNames));

            ExcludeProperties(propertyNames);

            return this;
        }

        /// <summary>Filters the current collection of properties based on the specified <paramref name="predicate"/>.
        /// If <see cref="Including(string[])"/> or <see cref="Excluding(string[])"/> has not been called, then the
        /// predicate is applied against all properties on the class type.</summary>
        /// <param name="predicate">The predicate to filter the properties.</param>
        /// <returns>The current instance to cater for a fluent syntax.</returns>
        public ClassProperties Where(Func<PropertyInfo, bool> predicate)
        {
            _ = predicate.WhenNotNull(nameof(predicate));

            AddFilter(predicate);

            return this;
        }
    }
}
