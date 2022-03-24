using System;
using System.Collections.Generic;
using System.Reflection;
using AllOverIt.Caching;

namespace AllOverIt.Reflection.Extensions
{
    public static class GenericCacheExtensions
    {
        public static IEnumerable<PropertyInfo> GetPropertyInfo(this GenericCache cache, Type type, BindingOptions bindingOptions, bool declaredOnly,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver = default)
        {
            return cache.GetOrAdd(new GenericCacheKey<Type, BindingOptions, bool>(type, bindingOptions, declaredOnly), valueResolver);
        }

        public static IEnumerable<PropertyInfo> GetPropertyInfo(this GenericCache cache, TypeInfo typeInfo, bool declaredOnly,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver = default)
        {
            return cache.GetOrAdd(new GenericCacheKey<TypeInfo, bool>(typeInfo, declaredOnly), valueResolver);
        }

        public static PropertyInfo GetPropertyInfo(this GenericCache cache, TypeInfo typeInfo, string propertyName,
            Func<GenericCacheKeyBase, PropertyInfo> valueResolver = default)
        {
            return cache.GetOrAdd(new GenericCacheKey<TypeInfo, string>(typeInfo, propertyName), valueResolver);
        }
    }
}