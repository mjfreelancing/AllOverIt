using System;
using System.Collections.Generic;
using System.Reflection;
using AllOverIt.Cache;
using AllOverIt.Extensions;
using AllOverIt.Reflection.Extensions;

namespace AllOverIt.Reflection
{
    public static class ReflectionCache
    {
        public static IEnumerable<PropertyInfo> GetPropertyInfo(Type type, BindingOptions bindingOptions, bool declaredOnly,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver = default)
        {
            valueResolver ??= key =>
            {
                var (keyType, keyBinding, keyDeclaredOnly) = (GenericCacheKey<Type, BindingOptions, bool>) key;

                return keyType
                    .GetPropertyInfo(keyBinding, keyDeclaredOnly)
                    .AsReadOnlyCollection();
            };

            return GenericCache.Default.GetPropertyInfo(type, bindingOptions, declaredOnly, valueResolver);
        }

        public static IEnumerable<PropertyInfo> GetPropertyInfo(TypeInfo typeInfo, bool declaredOnly, Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver)
        {
            valueResolver ??= key =>
            {
                var (keyTypeInfo, keyDeclaredOnly) = (GenericCacheKey<TypeInfo, bool>) key;

                return keyTypeInfo
                    .GetPropertyInfo(keyDeclaredOnly)
                    .AsReadOnlyCollection();
            };

            return GenericCache.Default.GetPropertyInfo(typeInfo, declaredOnly, valueResolver);
        }

        public static PropertyInfo GetPropertyInfo(TypeInfo typeInfo, string propertyName, Func<GenericCacheKeyBase, PropertyInfo> valueResolver)
        {
            valueResolver ??= key =>
            {
                var (keyTypeInfo, keyPropertyName) = (GenericCacheKey<TypeInfo, string>) key;

                return keyTypeInfo.GetPropertyInfo(keyPropertyName);
            };

            return GenericCache.Default.GetPropertyInfo(typeInfo, propertyName, valueResolver);
        }
    }
}