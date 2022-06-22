using System;
using System.Collections.Generic;
using System.Reflection;
using AllOverIt.Caching;

namespace AllOverIt.Reflection.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="GenericCache"/>.</summary>
    public static partial class GenericCacheExtensions
    {
        /// <summary>Gets the <see cref="PropertyInfo"/> (property metadata) for a given property on a <typeparamref name="TType"/>.</summary>
        /// <param name="cache">The cache to get the <see cref="PropertyInfo"/> from.</param>
        /// <param name="type">The type to get the <see cref="PropertyInfo"/> for.</param>
        /// <param name="propertyName">The name of the property to obtain metadata for.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a specified property on the specified <paramref name="type"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <paramref name="type"/>.</remarks>
        public static PropertyInfo GetPropertyInfo(this GenericCache cache, Type type, string propertyName, Func<GenericCacheKeyBase, PropertyInfo> valueResolver)
        {
            var key = new GenericCacheKey<ReflectionCacheKey<PropertyInfo>, Type, string>(null, type, propertyName);

            return cache.GetOrAdd(key, valueResolver);
        }

        /// <summary>Gets the <see cref="PropertyInfo"/> for a given property on a <see cref="TypeInfo"/> from the specified cache. If the
        /// <see cref="PropertyInfo"/> is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the
        /// cache before returning.</summary>
        /// <param name="cache">The cache to get the <see cref="PropertyInfo"/> from.</param>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to get the <see cref="PropertyInfo"/> for.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/> for a given property on a <see cref="TypeInfo"/> from the specified cache.</returns>
        public static PropertyInfo GetPropertyInfo(this GenericCache cache, TypeInfo typeInfo, string propertyName,
            Func<GenericCacheKeyBase, PropertyInfo> valueResolver)
        {
            var key = new GenericCacheKey<ReflectionCacheKey<PropertyInfo>, TypeInfo, string>(null, typeInfo, propertyName);

            return cache.GetOrAdd(key, valueResolver);
        }

        /// <summary>Gets all <see cref="PropertyInfo"/> for a given <see cref="Type"/> and options from the specified cache. If the <see cref="PropertyInfo"/>
        /// is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the cache before returning.</summary>
        /// <param name="cache">The cache to get the <see cref="PropertyInfo"/> from.</param>
        /// <param name="type">The type to get the <see cref="PropertyInfo"/> for.</param>
        /// <param name="bindingOptions">The binding option that determines the scope, access, and visibility rules to apply when searching for the <see cref="PropertyInfo"/>.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned (if a property is
        /// overriden then only the base class <see cref="PropertyInfo"/> is returned). If false, only property metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/> for a given <see cref="Type"/> and options from the specified cache.</returns>
        public static IEnumerable<PropertyInfo> GetPropertyInfo(this GenericCache cache, Type type, BindingOptions bindingOptions, bool declaredOnly,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver)
        {
            var key = new GenericCacheKey<ReflectionCacheKey<PropertyInfo>, Type, BindingOptions, bool>(null, type, bindingOptions, declaredOnly);
            
            return cache.GetOrAdd(key, valueResolver);
        }

        /// <summary>Gets all <see cref="PropertyInfo"/> for a given <see cref="Type"/> and options from the specified cache. If the <see cref="PropertyInfo"/>
        /// is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the cache before returning.</summary>
        /// <param name="cache">The cache to get the <see cref="PropertyInfo"/> from.</param>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to get the <see cref="PropertyInfo"/> for.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned (if a property is
        /// overriden then only the base class <see cref="PropertyInfo"/> is returned). If false, only property metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/> for a given <see cref="TypeInfo"/> and options from the specified cache.</returns>
        public static IEnumerable<PropertyInfo> GetPropertyInfo(this GenericCache cache, TypeInfo typeInfo, bool declaredOnly,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver)
        {
            var key = new GenericCacheKey<ReflectionCacheKey<PropertyInfo>, TypeInfo, bool>(null, typeInfo, declaredOnly);

            return cache.GetOrAdd(key, valueResolver);
        }
    }
}