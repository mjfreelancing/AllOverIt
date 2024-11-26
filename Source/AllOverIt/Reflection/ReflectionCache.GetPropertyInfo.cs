using AllOverIt.Assertion;
using AllOverIt.Caching;
using AllOverIt.Extensions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    /// <summary>Provides a default, static, cache to help improve performance where reflection is used extensively.</summary>
    public static partial class ReflectionCache
    {
        private static readonly GenericCache PropertyInfoCache = [];

        /// <summary>Gets the <see cref="PropertyInfo"/> (property metadata) for a given property on a <typeparamref name="TType"/> from the default cache.
        /// If the <see cref="PropertyInfo"/> is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the
        /// cache before returning.</summary>
        /// <typeparam name="TType">The type to obtain the property metadata from.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a specified property on the specified <typeparamref name="TType"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <typeparamref name="TType"/>.</remarks>
        public static PropertyInfo? GetPropertyInfo<TType>(string propertyName, Func<GenericCacheKeyBase, PropertyInfo>? valueResolver = default)
        {
            _ = propertyName.WhenNotNullOrEmpty();

            var typeInfo = typeof(TType).GetTypeInfo();

            return GetPropertyInfo(typeInfo, propertyName, valueResolver);
        }

        /// <summary>Gets the <see cref="PropertyInfo"/> for a given property on a <see cref="Type"/> from the default cache. If the <see cref="PropertyInfo"/>
        /// is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the cache before returning.</summary>
        /// <param name="type"></param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/> for a given property on a <see cref="Type"/> from the default cache.</returns>
        public static PropertyInfo? GetPropertyInfo(Type type, string propertyName, Func<GenericCacheKeyBase, PropertyInfo>? valueResolver = default)
        {
            _ = type.WhenNotNull();
            _ = propertyName.WhenNotNullOrEmpty();

            return GetPropertyInfo(type.GetTypeInfo(), propertyName, valueResolver);
        }

        /// <summary>Gets the <see cref="PropertyInfo"/> for a given property on a <see cref="TypeInfo"/> from the default cache. If the
        /// <see cref="PropertyInfo"/> is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the
        /// cache before returning.</summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to get the <see cref="PropertyInfo"/> for.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/> for a given property on a <see cref="TypeInfo"/> from the default cache.</returns>
        public static PropertyInfo? GetPropertyInfo(TypeInfo typeInfo, string propertyName, Func<GenericCacheKeyBase, PropertyInfo>? valueResolver = default)
        {
            _ = typeInfo.WhenNotNull();
            _ = propertyName.WhenNotNullOrEmpty();

            var key = new GenericCacheKey<TypeInfo, string>(typeInfo, propertyName);

            return PropertyInfoCache.GetOrAdd(key, valueResolver ?? GetPropertyInfoFromTypeInfoPropertyName());
        }

        /// <summary>Gets <see cref="PropertyInfo"/> (property metadata) for a given <typeparamref name="TType"/> and options from the default cache.
        /// If the <see cref="PropertyInfo"/> is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the
        /// cache before returning.</summary>
        /// <typeparam name="TType">The type to obtain property metadata for.</typeparam>
        /// <param name="bindingOptions">The binding option that determines the scope, access, and visibility rules to apply when searching for the <see cref="PropertyInfo"/>.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If false, only property metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>.</param>
        /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a specified <typeparamref name="TType"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
        /// by <typeparamref name="TType"/>.</remarks>
        public static IEnumerable<PropertyInfo>? GetPropertyInfo<TType>(BindingOptions bindingOptions = BindingOptions.Default, bool declaredOnly = false,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>>? valueResolver = default)
        {
            return GetPropertyInfo(typeof(TType), bindingOptions, declaredOnly, valueResolver);
        }

        /// <summary>Gets all <see cref="PropertyInfo"/> for a given <see cref="Type"/> and options from the default cache. If the <see cref="PropertyInfo"/>
        /// is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the cache before returning.</summary>
        /// <param name="type">The type to get the <see cref="PropertyInfo"/> for.</param>
        /// <param name="bindingOptions">The binding option that determines the scope, access, and visibility rules to apply when searching for the <see cref="PropertyInfo"/>.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned (if a property is
        /// overridden then only the base class <see cref="PropertyInfo"/> is returned). If false, only property metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/> for a given <see cref="Type"/> and options from the default cache.</returns>
        public static IEnumerable<PropertyInfo>? GetPropertyInfo(Type type, BindingOptions bindingOptions = BindingOptions.Default, bool declaredOnly = false,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>>? valueResolver = default)
        {
            _ = type.WhenNotNull();

            var key = new GenericCacheKey<Type, BindingOptions, bool>(type, bindingOptions, declaredOnly);

            return PropertyInfoCache.GetOrAdd(key, valueResolver ?? GetPropertyInfoFromTypeBindingDeclaredOnly());
        }

        /// <summary>Gets all <see cref="PropertyInfo"/> for a given <see cref="Type"/> and options from the default cache. If the <see cref="PropertyInfo"/>
        /// is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the cache before returning.</summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to get the <see cref="PropertyInfo"/> for.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned (if a property is
        /// overridden then only the base class <see cref="PropertyInfo"/> is returned). If false, only property metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="PropertyInfo"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/> for a given <see cref="TypeInfo"/> and options from the default cache.</returns>
        public static IEnumerable<PropertyInfo>? GetPropertyInfo(TypeInfo typeInfo, bool declaredOnly = false,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>>? valueResolver = default)
        {
            _ = typeInfo.WhenNotNull();

            var key = new GenericCacheKey<TypeInfo, bool>(typeInfo, declaredOnly);

            return PropertyInfoCache.GetOrAdd(key, valueResolver ?? GetPropertyInfoFromTypeInfoDeclaredOnly());
        }

        private static Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> GetPropertyInfoFromTypeBindingDeclaredOnly()
        {
            return key =>
            {
                var (type, bindingOptions, declaredOnly) = (GenericCacheKey<Type, BindingOptions, bool>) key;

                return type!
                    .GetPropertyInfo(bindingOptions, declaredOnly)
                    .ToArray();
            };
        }

        private static Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> GetPropertyInfoFromTypeInfoDeclaredOnly()
        {
            return key =>
            {
                var (typeInfo, declaredOnly) = (GenericCacheKey<TypeInfo, bool>) key;

                return typeInfo!
                    .GetPropertyInfo(declaredOnly)
                    .ToArray();
            };
        }

        private static Func<GenericCacheKeyBase, PropertyInfo> GetPropertyInfoFromTypeInfoPropertyName()
        {
            return key =>
            {
                var (typeInfo, propertyName) = (GenericCacheKey<TypeInfo, string>) key;

                return typeInfo!.GetPropertyInfo(propertyName!)!;
            };
        }
    }
}