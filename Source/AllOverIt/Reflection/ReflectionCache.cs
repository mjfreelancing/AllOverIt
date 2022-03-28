using System;
using System.Collections.Generic;
using System.Reflection;
using AllOverIt.Caching;
using AllOverIt.Extensions;
using AllOverIt.Reflection.Extensions;

namespace AllOverIt.Reflection
{
    public static class ReflectionCache
    {
        /// <summary>Gets all <see cref="PropertyInfo"/> for a given <see cref="Type"/> and options from the default cache. If the <see cref="PropertyInfo"/>
        /// is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the cache before returning.</summary>
        /// <param name="type">The type to get the <see cref="PropertyInfo"/> for.</param>
        /// <param name="bindingOptions">The binding option that determines the scope, access, and visibility rules to apply when searching for the <see cref="PropertyInfo"/>.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned (if a property is
        /// overriden then only the base class <see cref="PropertyInfo"/> is returned). If false, only property metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>. If null, the information
        /// will be obtained using <seealso cref="AllOverIt.Extensions.TypeExtensions.GetPropertyInfo(Type, BindingOptions, bool)"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/> for a given <see cref="Type"/> and options from the default cache.</returns>
        public static IEnumerable<PropertyInfo> GetPropertyInfo(Type type, BindingOptions bindingOptions, bool declaredOnly = false,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver = default)
        {
            return GenericCache.Default.GetPropertyInfo(type, bindingOptions, declaredOnly, valueResolver ?? GetPropertyInfoFromTypeBindingDeclaredOnly());
        }

        /// <summary>Gets all <see cref="PropertyInfo"/> for a given <see cref="Type"/> and options from the default cache. If the <see cref="PropertyInfo"/>
        /// is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the cache before returning.</summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to get the <see cref="PropertyInfo"/> for.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned (if a property is
        /// overriden then only the base class <see cref="PropertyInfo"/> is returned). If false, only property metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>. If null, the information
        /// will be obtained using <seealso cref="AllOverIt.Extensions.TypeInfoExtensions.GetPropertyInfo(TypeInfo, bool)"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/> for a given <see cref="TypeInfo"/> and options from the default cache.</returns>
        public static IEnumerable<PropertyInfo> GetPropertyInfo(TypeInfo typeInfo, bool declaredOnly = false,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver = default)
        {
            return GenericCache.Default.GetPropertyInfo(typeInfo, declaredOnly, valueResolver ?? GetPropertyInfoFromTypeInfoDeclaredOnly());
        }

        /// <summary>Gets the <see cref="PropertyInfo"/> for a given property on a <see cref="TypeInfo"/> from the default cache. If the
        /// <see cref="PropertyInfo"/> is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the
        /// cache before returning.</summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to get the <see cref="PropertyInfo"/> for.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>. If null, the information
        /// will be obtained using <seealso cref="AllOverIt.Extensions.TypeInfoExtensions.GetPropertyInfo(TypeInfo, string)"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/> for a given property on a <see cref="TypeInfo"/> from the default cache.</returns>
        public static PropertyInfo GetPropertyInfo(TypeInfo typeInfo, string propertyName, Func<GenericCacheKeyBase, PropertyInfo> valueResolver = default)
        {
            return GenericCache.Default.GetPropertyInfo(typeInfo, propertyName, valueResolver ?? GetPropertyInfoFromTypeInfoPropertyName());
        }

        private static Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> GetPropertyInfoFromTypeBindingDeclaredOnly()
        {
            return key =>
            {
                var (type, bindingOptions, declaredOnly) = (GenericCacheKey<Type, BindingOptions, bool>) key;

                return type
                    .GetPropertyInfo(bindingOptions, declaredOnly)
                    .AsReadOnlyCollection();
            };
        }

        private static Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> GetPropertyInfoFromTypeInfoDeclaredOnly()
        {
            return key =>
            {
                var (typeInfo, declaredOnly) = (GenericCacheKey<TypeInfo, bool>) key;

                return typeInfo
                    .GetPropertyInfo(declaredOnly)
                    .AsReadOnlyCollection();
            };
        }

        private static Func<GenericCacheKeyBase, PropertyInfo> GetPropertyInfoFromTypeInfoPropertyName()
        {
            return key =>
            {
                var (typeInfo, propertyName) = (GenericCacheKey<TypeInfo, string>) key;

                return typeInfo.GetPropertyInfo(propertyName);
            };
        }
    }
}