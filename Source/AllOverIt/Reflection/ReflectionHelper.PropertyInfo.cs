using AllOverIt.Extensions;
using System.Collections.Generic;
using System.Reflection;

namespace AllOverIt.Reflection
{
    /// <summary>Contains a number of helper functions related to reflection.</summary>
    public static partial class ReflectionHelper
    {
        /// <summary>Gets the <see cref="PropertyInfo"/> (property metadata) for a given property on a <typeparamref name="TType"/>.</summary>
        /// <typeparam name="TType">The type to obtain the property metadata from.</typeparam>
        /// <param name="propertyName">The name of the property to obtain metadata for.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a specified property on the specified <typeparamref name="TType"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <typeparamref name="TType"/>.</remarks>
        public static PropertyInfo GetPropertyInfo<TType>(string propertyName)
        {
            return typeof(TType).GetPropertyInfo(propertyName);
        }

        /// <summary>Gets <see cref="PropertyInfo"/> (property metadata) for a given <typeparamref name="TType"/> and binding option.</summary>
        /// <typeparam name="TType">The type to obtain property metadata for.</typeparam>
        /// <param name="binding">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If false, only property metadata of the declared type is returned.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a specified <typeparamref name="TType"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <typeparamref name="TType"/>.</remarks>
        public static IEnumerable<PropertyInfo> GetPropertyInfo<TType>(BindingOptions binding = BindingOptions.Default, bool declaredOnly = false)
        {
            return typeof(TType).GetPropertyInfo(binding, declaredOnly);
        }
    }
}
