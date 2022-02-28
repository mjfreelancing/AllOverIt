using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;

namespace AllOverIt.Mapping
{
    internal static class ObjectMapperHelper
    {
        internal static IReadOnlyCollection<PropertyInfo> GetMappableProperties(Type sourceType, Type targetType, BindingOptions bindingOptions)
        {
            var sourceProps = sourceType.GetPropertyInfo(bindingOptions).Where(prop => prop.CanRead);
            var destProps = targetType.GetPropertyInfo(bindingOptions).Where(prop => prop.CanWrite);

            return sourceProps
                .FindMatches(
                    destProps,
                    src => $"{src.Name}.{src.PropertyType.GetFriendlyName()}",
                    target => $"{target.Name}.{target.PropertyType.GetFriendlyName()}")
                .AsReadOnlyCollection();
        }

        internal static void MapPropertyValues(Type sourceType, object source, Type targetType, object target, IReadOnlyCollection<PropertyInfo> matches,
            ObjectMapperOptions options)
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(source));
            _ = matches.WhenNotNull(nameof(matches));       // allow empty
            _ = options.WhenNotNull(nameof(options));

            // see if any properties need filtering out
            if (options.Filter != null)
            {
                matches = matches.Where(options.Filter).AsReadOnlyCollection();
            }

            MapPropertyValues(sourceType, source, targetType, target, matches, options.Binding);
        }

        internal static void MapPropertyValues(Type sourceType, object source, Type targetType, object target, IReadOnlyCollection<PropertyInfo> matches,
            BindingOptions bindingOptions)
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(source));
            _ = matches.WhenNotNull(nameof(matches));       // allow empty

            foreach (var match in matches)
            {
                var value = source.GetPropertyValue(sourceType, match.Name, bindingOptions);
                target.SetPropertyValue(targetType, match.Name, value, bindingOptions);
            }
        }
    }
}