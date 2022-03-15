using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AllOverIt.Mapping.Extensions;

namespace AllOverIt.Mapping
{
    internal static class ObjectMapperHelper
    {
        internal static IReadOnlyCollection<PropertyInfo> GetMappableProperties(Type sourceType, Type targetType, ObjectMapperOptions options)
        {
            _ = options.WhenNotNull(nameof(options));

            static IReadOnlyCollection<PropertyInfo> GetMappablePropertyInfo(ReflectionCacheKeyBase key)
            {
                var (type, binding, declaredOnly) = (ReflectionCacheKey<Type, BindingOptions, bool>) key;
                return type.GetPropertyInfo(binding, declaredOnly).AsReadOnlyCollection();
            }

            var sourceProps = ReflectionCache.Instance.GetPropertyInfo(sourceType, options.Binding, false, GetMappablePropertyInfo).Where(prop => prop.CanRead && !options.IsExcluded(prop.Name));
            var destProps = ReflectionCache.Instance.GetPropertyInfo(targetType, options.Binding, false, GetMappablePropertyInfo).Where(prop => prop.CanWrite);

            // Apart from a performance benefit, the source properties must be filtered before looking for matches just in case the source
            // contains a property name that is not required (excluded via the Filter) but is mapped to a target property of the same name.
            // Without the pre-filtering, the source selector used in FindMatches() would result in that property name being added twice,
            // resulting in a duplicate key error.
            if (options.Filter != null)
            {
                sourceProps = sourceProps.Where(options.Filter);
            }

            return sourceProps
                .FindMatches(
                    destProps,
                    src => GetTargetAliasName(src.Name, options),
                    target => target.Name)
                .AsReadOnlyCollection();
        }

        // Only to be used when property values need to be get/set based on binding options (ie., static methods, never ObjectMapper)
        internal static void MapPropertyValues(Type sourceType, object source, Type targetType, object target, IReadOnlyCollection<PropertyInfo> matches,
            ObjectMapperOptions options)
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(target));
            _ = matches.WhenNotNull(nameof(matches));                   // allow empty
            _ = options.WhenNotNull(nameof(options));

            foreach (var match in matches)
            {
                var value = source.GetPropertyValue(sourceType, match.Name, options.Binding);
                var targetName = GetTargetAliasName(match.Name, options);
                var targetValue = options.GetConvertedValue(match.Name, value);

                target.SetPropertyValue(targetType, targetName, targetValue, options.Binding);
            }
        }

        internal static string GetTargetAliasName(string sourceName, ObjectMapperOptions options)
        {
            return options.GetAliasName(sourceName) ?? sourceName;
        }
    }
}