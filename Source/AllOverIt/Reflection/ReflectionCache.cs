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
        public static IEnumerable<PropertyInfo> GetPropertyInfo(Type type, BindingOptions bindingOptions, bool declaredOnly = false,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver = default)
        {
            return GenericCache.Default.GetPropertyInfo(type, bindingOptions, declaredOnly, valueResolver ?? GetPropertyInfoFromTypeBindingDeclaredOnly());
        }

        public static IEnumerable<PropertyInfo> GetPropertyInfo(TypeInfo typeInfo, bool declaredOnly = false,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver = default)
        {
            return GenericCache.Default.GetPropertyInfo(typeInfo, declaredOnly, valueResolver ?? GetPropertyInfoFromTypeInfoDeclaredOnly());
        }

        public static PropertyInfo GetPropertyInfo(TypeInfo typeInfo, string propertyName,
            Func<GenericCacheKeyBase, PropertyInfo> valueResolver = default)
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