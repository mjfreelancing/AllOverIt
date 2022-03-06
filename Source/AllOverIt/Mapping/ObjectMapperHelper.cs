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

            static IReadOnlyCollection<PropertyInfo> GetMappablePropertyInfo(ReflectionCacheKeyBase key)
            {
                var (t, b, d) = (ReflectionCacheKey<Type, BindingOptions, bool>) key;
                return t.GetPropertyInfo(b, d).AsReadOnlyCollection();
            }

            var sourceProps = ReflectionCache.Instance.GetPropertyInfo(sourceType, bindingOptions, false, GetMappablePropertyInfo).Where(prop => prop.CanRead);
            var destProps = ReflectionCache.Instance.GetPropertyInfo(targetType, bindingOptions, false, GetMappablePropertyInfo).Where(prop => prop.CanWrite);

            return sourceProps
                .FindMatches(
                    destProps,
                    src =>
                    {
                        // returns src.Name is there's no matching alias, or aliases is null
                        var aliasName = GetTargetAliasName(src.Name, aliases);
                        return (aliasName, src.PropertyType);
                    },
                    target => (target.Name, target.PropertyType))
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

            MapPropertyValues(sourceType, source, targetType, target, matches, options.Binding, options.Aliases);
        }

        internal static void MapPropertyValues(Type sourceType, object source, Type targetType, object target, IReadOnlyCollection<PropertyInfo> matches,
            BindingOptions bindingOptions, IDictionary<string, string> aliases = default)
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(target));
            _ = matches.WhenNotNull(nameof(matches));       // allow empty

            foreach (var match in matches)
            {
                var value = source.GetPropertyValue(sourceType, match.Name, bindingOptions);
                var targetName = GetTargetAliasName(match.Name, aliases);
                target.SetPropertyValue(targetType, targetName, value, bindingOptions);
            }
        }

        internal static string GetTargetAliasName(string sourceName, IDictionary<string, string> aliasLookup)
        {
            return aliasLookup?.GetValueOrDefault(sourceName, sourceName) ?? sourceName;
        }
    }
}