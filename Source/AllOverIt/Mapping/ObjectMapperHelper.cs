using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Mapping
{
    internal static class ObjectMapperHelper
    {
        internal static IReadOnlyCollection<PropertyInfo> GetMappableProperties(Type sourceType, Type targetType, ObjectMapperOptions options)
        {
            _ = options.WhenNotNull(nameof(options));

            static IReadOnlyCollection<PropertyInfo> GetMappablePropertyInfo(ReflectionCacheKeyBase key)
            {
                var (t, b, d) = (ReflectionCacheKey<Type, BindingOptions, bool>) key;
                return t.GetPropertyInfo(b, d).AsReadOnlyCollection();
            }

            var sourceProps = ReflectionCache.Instance.GetPropertyInfo(sourceType, options.Binding, false, GetMappablePropertyInfo).Where(prop => prop.CanRead);
            var destProps = ReflectionCache.Instance.GetPropertyInfo(targetType, options.Binding, false, GetMappablePropertyInfo).Where(prop => prop.CanWrite);

            return sourceProps
                .FindMatches(
                    destProps,
                    src =>
                    {
                        // returns src.Name is there's no matching alias, or aliases is null
                        var aliasName = GetTargetAliasName(src.Name, options);
                        return (aliasName, GetUnderlyingPropertyType(src));
                    },
                    target => (target.Name, GetUnderlyingPropertyType(target))
                )
                .AsReadOnlyCollection();
        }

        internal static void MapPropertyValues(Type sourceType, object source, Type targetType, object target, IReadOnlyCollection<PropertyInfo> matches,
            ObjectMapperOptions options)
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(target));
            _ = matches.WhenNotNull(nameof(matches));       // allow empty
            _ = options.WhenNotNull(nameof(options));

            // see if any properties need filtering out
            if (options.Filter != null)
            {
                matches = matches.Where(options.Filter).AsReadOnlyCollection();
            }

            foreach (var match in matches)
            {
                var value = source.GetPropertyValue(sourceType, match.Name, options.Binding);
                var targetName = GetTargetAliasName(match.Name, options);
                target.SetPropertyValue(targetType, targetName, value, options.Binding);
            }
        }

        internal static string GetTargetAliasName(string sourceName, ObjectMapperOptions options)
        {
            return options.GetAlias(sourceName) ?? sourceName;
        }

        private static Type GetUnderlyingPropertyType(PropertyInfo propInfo)
        {
            return Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
        }
    }
}