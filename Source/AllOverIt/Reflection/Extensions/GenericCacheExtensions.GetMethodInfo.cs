using System;
using System.Collections.Generic;
using System.Reflection;
using AllOverIt.Caching;

namespace AllOverIt.Reflection.Extensions
{
    public static partial class GenericCacheExtensions
    {
        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <paramref name="type"/> method with a given name and argument types.</summary>
        /// <param name="cache">The cache to get the <see cref="PropertyInfo"/> from.</param>
        /// <param name="type">The type to obtain method metadata for.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="types">The argument types expected on the method</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="MethodInfo"/>.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a specified <paramref name="type"/> with a given name and argument types.</returns>
        /// <remarks>All instance, static, public, and non-public methods are searched.</remarks>
        public static MethodInfo GetMethodInfo(this GenericCache cache, Type type, string name, Type[] types, Func<GenericCacheKeyBase, MethodInfo> valueResolver)
        {
            var key = new GenericCacheKey<ReflectionCacheKey<MethodInfo>, Type, string, Type[]>(null, type, name, types);

            return cache.GetOrAdd(key, valueResolver);
        }

        /// <summary>Gets <see cref="MethodInfo"/> (method metadata) for a given <paramref name="type"/> and binding options.</summary>
        /// <param name="cache">The cache to get the <see cref="PropertyInfo"/> from.</param>
        /// <param name="type">The type to obtain method metadata for.</param>
        /// <param name="bindingOptions">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If false, only method metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="MethodInfo"/>.</param>
        /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a specified <paramref name="type"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first method found, starting at the type represented
        /// by  <paramref name="type"/>.</remarks>
        public static IEnumerable<MethodInfo> GetMethodInfo(this GenericCache cache, Type type, BindingOptions bindingOptions, bool declaredOnly,
            Func<GenericCacheKeyBase, IEnumerable<MethodInfo>> valueResolver)
        {
            var key = new GenericCacheKey<ReflectionCacheKey<MethodInfo>, Type, BindingOptions, bool>(null, type, bindingOptions, declaredOnly);

            return cache.GetOrAdd(key, valueResolver);
        }
    }
}