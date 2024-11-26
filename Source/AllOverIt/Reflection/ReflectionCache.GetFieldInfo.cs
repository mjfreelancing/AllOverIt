using AllOverIt.Assertion;
using AllOverIt.Caching;
using AllOverIt.Extensions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    /// <summary>Provides a default, static, cache to help improve performance where reflection is used extensively.</summary>
    public static partial class ReflectionCache
    {
        private static readonly GenericCache FieldInfoCache = [];

        /// <summary>Gets the <see cref="FieldInfo"/> (field metadata) for a given field on a <typeparamref name="TType"/> from the default cache.
        /// If the <see cref="FieldInfo"/> is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the
        /// cache before returning.</summary>
        /// <typeparam name="TType">The type to obtain the field metadata from.</typeparam>
        /// <param name="fieldName">The field name.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="FieldInfo"/>.</param>
        /// <returns>The field metadata, as <see cref="FieldInfo"/>, of a specified field on the specified <typeparamref name="TType"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first field found, starting at the type represented
        /// by <typeparamref name="TType"/>.</remarks>
        public static FieldInfo? GetFieldInfo<TType>(string fieldName, Func<GenericCacheKeyBase, FieldInfo>? valueResolver = default)
        {
            _ = fieldName.WhenNotNullOrEmpty();

            var typeInfo = typeof(TType).GetTypeInfo();

            return GetFieldInfo(typeInfo, fieldName, valueResolver);
        }

        /// <summary>Gets the <see cref="PropertyInfo"/> for a given field on a <see cref="Type"/> from the default cache. If the <see cref="FieldInfo"/>
        /// is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the cache before returning.</summary>
        /// <param name="type"></param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="FieldInfo"/>.</param>
        /// <returns>The <see cref="FieldInfo"/> for a given field on a <see cref="Type"/> from the default cache.</returns>
        public static FieldInfo? GetFieldInfo(Type type, string fieldName, Func<GenericCacheKeyBase, FieldInfo>? valueResolver = default)
        {
            _ = type.WhenNotNull();
            _ = fieldName.WhenNotNullOrEmpty();

            return GetFieldInfo(type.GetTypeInfo(), fieldName, valueResolver);
        }

        /// <summary>Gets the <see cref="FieldInfo"/> for a given field on a <see cref="TypeInfo"/> from the default cache. If the
        /// <see cref="FieldInfo"/> is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the
        /// cache before returning.</summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to get the <see cref="FieldInfo"/> for.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="FieldInfo"/>.</param>
        /// <returns>The <see cref="FieldInfo"/> for a given field on a <see cref="TypeInfo"/> from the default cache.</returns>
        public static FieldInfo? GetFieldInfo(TypeInfo typeInfo, string fieldName, Func<GenericCacheKeyBase, FieldInfo>? valueResolver = default)
        {
            _ = typeInfo.WhenNotNull();
            _ = fieldName.WhenNotNullOrEmpty();

            var key = new GenericCacheKey<TypeInfo, string>(typeInfo, fieldName);

            return FieldInfoCache.GetOrAdd(key, valueResolver ?? GetFieldInfoFromTypeInfoPropertyName());
        }

        /// <summary>Gets <see cref="FieldInfo"/> (field metadata) for a given <typeparamref name="TType"/> and options from the default cache.
        /// If the <see cref="FieldInfo"/> is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the
        /// cache before returning.</summary>
        /// <typeparam name="TType">The type to obtain field metadata for.</typeparam>
        /// <param name="bindingOptions">The binding option that determines the scope, access, and visibility rules to apply when searching for the <see cref="FieldInfo"/>.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
        /// If false, only field metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="FieldInfo"/>.</param>
        /// <returns>The field metadata, as <see cref="FieldInfo"/>, of a specified <typeparamref name="TType"/>.</returns>
        /// <remarks>When class inheritance is involved, this method returns the first field found, starting at the type represented
        /// by <typeparamref name="TType"/>.</remarks>
        public static IEnumerable<FieldInfo>? GetFieldInfo<TType>(BindingOptions bindingOptions = BindingOptions.Default, bool declaredOnly = false,
            Func<GenericCacheKeyBase, IEnumerable<FieldInfo>>? valueResolver = default)
        {
            return GetFieldInfo(typeof(TType), bindingOptions, declaredOnly, valueResolver);
        }

        /// <summary>Gets all <see cref="FieldInfo"/> for a given <see cref="Type"/> and options from the default cache. If the <see cref="FieldInfo"/>
        /// is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the cache before returning.</summary>
        /// <param name="type">The type to get the <see cref="FieldInfo"/> for.</param>
        /// <param name="bindingOptions">The binding option that determines the scope, access, and visibility rules to apply when searching for the <see cref="FieldInfo"/>.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned (if a field is
        /// overridden then only the base class <see cref="FieldInfo"/> is returned). If false, only field metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="FieldInfo"/>.</param>
        /// <returns>The <see cref="FieldInfo"/> for a given <see cref="Type"/> and options from the default cache.</returns>
        public static IEnumerable<FieldInfo>? GetFieldInfo(Type type, BindingOptions bindingOptions = BindingOptions.Default, bool declaredOnly = false,
            Func<GenericCacheKeyBase, IEnumerable<FieldInfo>>? valueResolver = default)
        {
            _ = type.WhenNotNull();

            var key = new GenericCacheKey<Type, BindingOptions, bool>(type, bindingOptions, declaredOnly);

            return FieldInfoCache.GetOrAdd(key, valueResolver ?? GetFieldInfoFromTypeBindingDeclaredOnly());
        }

        /// <summary>Gets all <see cref="FieldInfo"/> for a given <see cref="Type"/> and options from the default cache. If the <see cref="FieldInfo"/>
        /// is not in the cache then it will be obtained using the <paramref name="valueResolver"/> and added to the cache before returning.</summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to get the <see cref="FieldInfo"/> for.</param>
        /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned (if a field is
        /// overridden then only the base class <see cref="FieldInfo"/> is returned). If false, only field metadata of the declared type is returned.</param>
        /// <param name="valueResolver">The factory method to obtain the required <see cref="FieldInfo"/>.</param>
        /// <returns>The <see cref="FieldInfo"/> for a given <see cref="TypeInfo"/> and options from the default cache.</returns>
        public static IEnumerable<FieldInfo>? GetFieldInfo(TypeInfo typeInfo, bool declaredOnly = false,
            Func<GenericCacheKeyBase, IEnumerable<FieldInfo>>? valueResolver = default)
        {
            _ = typeInfo.WhenNotNull();

            var key = new GenericCacheKey<TypeInfo, bool>(typeInfo, declaredOnly);

            return FieldInfoCache.GetOrAdd(key, valueResolver ?? GetFieldInfoFromTypeInfoDeclaredOnly());
        }

        private static Func<GenericCacheKeyBase, IEnumerable<FieldInfo>> GetFieldInfoFromTypeBindingDeclaredOnly()
        {
            return key =>
            {
                var (type, bindingOptions, declaredOnly) = (GenericCacheKey<Type, BindingOptions, bool>) key;

                return type!
                    .GetFieldInfo(bindingOptions, declaredOnly)
                    .ToArray();
            };
        }

        private static Func<GenericCacheKeyBase, IEnumerable<FieldInfo>> GetFieldInfoFromTypeInfoDeclaredOnly()
        {
            return key =>
            {
                var (typeInfo, declaredOnly) = (GenericCacheKey<TypeInfo, bool>) key;

                return typeInfo!
                    .GetFieldInfo(declaredOnly)
                    .ToArray();
            };
        }

        private static Func<GenericCacheKeyBase, FieldInfo> GetFieldInfoFromTypeInfoPropertyName()
        {
            return key =>
            {
                var (typeInfo, propertyName) = (GenericCacheKey<TypeInfo, string>) key;

                return typeInfo!.GetFieldInfo(propertyName!)!;
            };
        }
    }
}