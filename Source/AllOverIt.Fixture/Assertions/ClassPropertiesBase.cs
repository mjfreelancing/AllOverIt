using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using FluentAssertions;
using System.Reflection;

namespace AllOverIt.Fixture.Assertions
{
    // Don't move ClassPropertiesExtensions - less troublesome to use if it has the same namespace as Properties.For<T>()

    /// <summary>Provides extension methods for the <see cref="ClassPropertiesAssertions"/>.</summary>
    public static class ClassPropertiesExtensions
    {
        /// <summary>Gets a <see cref="ClassPropertiesAssertions"/> that can be used to assert properties on a type.</summary>
        /// <param name="instance">The <see cref="ClassPropertiesBase"/> containing the property info to assert.</param>
        /// <returns>A <see cref="ClassPropertiesAssertions"/> that can be used to assert properties on a type.</returns>
        public static ClassPropertiesAssertions Should(this ClassPropertiesBase instance)
        {
            return new ClassPropertiesAssertions(instance);
        }
    }

    /// <summary>An abstract base class providing methods for filtering class properties that can be asserted.</summary>
    public abstract class ClassPropertiesBase
    {
        private IEnumerable<PropertyInfo>? _properties;

        private readonly PropertyInfo[] _allProperties;

        /// <summary>Gets the currently filtered property names.</summary>
        internal PropertyInfo[] Properties => [.. (_properties ?? _allProperties)];

        /// <summary>The class type that the property assertions are targeting.</summary>
        public Type ClassType { get; }

        /// <summary>Constructor.</summary>
        /// <param name="classType">The class type to assert its' property definitions.</param>
        /// <param name="declaredOnly">When <see langword="True"/>, base class properties will be ignored.</param>
        public ClassPropertiesBase(Type classType, bool declaredOnly)
        {
            ClassType = classType.WhenNotNull();

            _allProperties = [.. classType
                .GetPropertyInfo(BindingOptions.All, declaredOnly)
                .Where(propInfo => !propInfo.IsCompilerGenerated())];
        }

        /// <summary>Filters the properties to only those specified in <paramref name="propertyNames"/>. Property names
        /// that do not exist are ignored. Multiple calls to this method will further filter the results.</summary>
        /// <param name="propertyNames">The property names to be included.</param>
        protected void IncludeProperties(params string[] propertyNames)
        {
            _ = propertyNames.WhenNotNullOrEmpty();

            _properties = (_properties ?? _allProperties).Where(propInfo => propertyNames.Contains(propInfo.Name));
        }

        /// <summary>Filters the properties by excluding those specified in <paramref name="propertyNames"/>. Multiple calls
        /// to this method will further filter the results.</summary>
        /// <param name="propertyNames">The property names to be excluded.</param>
        protected void ExcludeProperties(params string[] propertyNames)
        {
            _ = propertyNames.WhenNotNullOrEmpty();

            _properties = (_properties ?? _allProperties).Where(propInfo => !propertyNames.Contains(propInfo.Name));
        }

        /// <summary>Filters the current collection of properties based on the specified <paramref name="predicate"/>.
        /// If <see cref="IncludeProperties(string[])"/> or <see cref="ExcludeProperties(string[])"/> has not been called,
        /// then the predicate is applied against all properties on the class type.</summary>
        /// <param name="predicate">The predicate to filter the properties.</param>
        protected void AddFilter(Func<PropertyInfo, bool> predicate)
        {
            _ = predicate.WhenNotNull();

            _properties = (_properties ?? _allProperties).Where(predicate);
        }
    }
}
