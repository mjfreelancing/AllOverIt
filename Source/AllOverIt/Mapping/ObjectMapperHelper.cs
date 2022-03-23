using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Mapping
{
    // These methods are indirectly tested via ObjectMapper and ObjectExtensions (AllOverIt.Mapping.Extensions)
    internal static class ObjectMapperHelper
    {
        internal static IReadOnlyCollection<PropertyInfo> GetMappableProperties(Type sourceType, Type targetType, ObjectMapperOptions options)
        {
            _ = options.WhenNotNull(nameof(options));

            var sourceProps = ReflectionCache.GetPropertyInfo(sourceType, options.Binding).Where(prop => prop.CanRead && !options.IsExcluded(prop.Name));
            var targetProps = ReflectionCache.GetPropertyInfo(targetType, options.Binding).Where(prop => prop.CanWrite);

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
                    targetProps,
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