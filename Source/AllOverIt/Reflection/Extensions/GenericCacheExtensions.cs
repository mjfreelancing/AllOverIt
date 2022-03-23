using System;
using System.Collections.Generic;
using System.Reflection;
using AllOverIt.Cache;

namespace AllOverIt.Reflection.Extensions
{
    public static class GenericCacheExtensions
    {
        public static Func<MethodBase, bool> GetBindingPredicate(this GenericCache cache, BindingOptions bindingOptions, Func<GenericCacheKeyBase, Func<MethodBase, bool>> valueResolver)
        {
            return cache.GetOrAdd<Func<MethodBase, bool>>(new GenericCacheKey<BindingOptions>(bindingOptions), valueResolver);
        }

        public static IEnumerable<PropertyInfo> GetPropertyInfo(this GenericCache cache, Type type, BindingOptions bindingOptions, bool declaredOnly,
            Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver)
        {
            return cache.GetOrAdd<IEnumerable<PropertyInfo>>(new GenericCacheKey<Type, BindingOptions, bool>(type, bindingOptions, declaredOnly), valueResolver);
        }

        public static IEnumerable<PropertyInfo> GetPropertyInfo(this GenericCache cache, TypeInfo typeInfo, bool declaredOnly, Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver)
        {
            return cache.GetOrAdd<IEnumerable<PropertyInfo>>(new GenericCacheKey<TypeInfo, bool>(typeInfo, declaredOnly), valueResolver);
        }

        public static PropertyInfo GetPropertyInfo(this GenericCache cache, TypeInfo typeInfo, string propertyName, Func<GenericCacheKeyBase, PropertyInfo> valueResolver)
        {
            return cache.GetOrAdd<PropertyInfo>(new GenericCacheKey<TypeInfo, string>(typeInfo, propertyName), valueResolver);
        }
    }
}