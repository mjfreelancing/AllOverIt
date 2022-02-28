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
        internal static IReadOnlyCollection<PropertyInfo> GetMappableProperties(Type sourceType, Type targetType, BindingOptions bindingOptions,
            IDictionary<string, string> aliases)
        {
            //_ = aliases.WhenNotNull(nameof(aliases));       // allow empty

            var sourceProps = sourceType.GetPropertyInfo(bindingOptions).Where(prop => prop.CanRead);
            var destProps = targetType.GetPropertyInfo(bindingOptions).Where(prop => prop.CanWrite);

            return sourceProps
                .FindMatches(
                    destProps,
                    src =>
                    {
                        var aliasName = GetTargetAliasName(src.Name, aliases);
                        return $"{aliasName}.{src.PropertyType.GetFriendlyName()}";
                    },
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

            MapPropertyValues(sourceType, source, targetType, target, matches, options.Binding, options.Aliases);
        }

        internal static void MapPropertyValues(Type sourceType, object source, Type targetType, object target, IReadOnlyCollection<PropertyInfo> matches,
            BindingOptions bindingOptions, IDictionary<string, string> aliases = default)
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(source));
            _ = matches.WhenNotNull(nameof(matches));       // allow empty

            foreach (var match in matches)
            {
                var value = source.GetPropertyValue(sourceType, match.Name, bindingOptions);
                var targetName = GetTargetAliasName(match.Name, aliases);
                target.SetPropertyValue(targetType, targetName, value, bindingOptions);
            }
        }

        private static string GetTargetAliasName(string sourceName, IDictionary<string, string> aliasLookup)
        {
            return aliasLookup?.GetValueOrDefault(sourceName, sourceName) ?? sourceName;
        }
    }
}