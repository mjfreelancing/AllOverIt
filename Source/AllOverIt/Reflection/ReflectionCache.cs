using System;
using System.Collections.Generic;
using System.Reflection;
using AllOverIt.Cache;
using AllOverIt.Reflection.Extensions;

namespace AllOverIt.Reflection
{
    public static class ReflectionCache
    {
        public static Func<MethodBase, bool> GetBindingPredicate(BindingOptions bindingOptions, Func<GenericCacheKeyBase, Func<MethodBase, bool>> valueResolver)
        {
            return GenericCache.Default.GetBindingPredicate(bindingOptions, valueResolver);
        }

        public static IEnumerable<PropertyInfo> GetPropertyInfo(Type type, BindingOptions bindingOptions, bool declaredOnly, Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver)
        {
            return GenericCache.Default.GetPropertyInfo(type, bindingOptions, declaredOnly, valueResolver);
        }

        public static IEnumerable<PropertyInfo> GetPropertyInfo(TypeInfo typeInfo, bool declaredOnly, Func<GenericCacheKeyBase, IEnumerable<PropertyInfo>> valueResolver)
        {
            return GenericCache.Default.GetPropertyInfo(typeInfo, declaredOnly, valueResolver);
        }

        public static PropertyInfo GetPropertyInfo(TypeInfo typeInfo, string propertyName, Func<GenericCacheKeyBase, PropertyInfo> valueResolver)
        {
            return GenericCache.Default.GetPropertyInfo(typeInfo, propertyName, valueResolver);
        }
    }
}