using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System.Reflection;

namespace AllOverIt.Mapping
{
    // These methods are indirectly tested via ObjectMapper and ObjectExtensions (AllOverIt.Mapping.Extensions)
    internal static class ObjectMapperHelper
    {
        internal static PropertyInfo[] GetMappableProperties(Type sourceType, Type targetType, PropertyMatcherOptions options)
        {
            _ = options.WhenNotNull();

            var sourceProps = GetFilteredSourcePropertyInfo(sourceType, options);
            var targetProps = GetFilteredTargetPropertyInfo(targetType, options);

            return [.. sourceProps
                .FindMatches(
                    targetProps,
                    src => GetTargetAliasName(src.Name, options),
                    target => target.Name)];
        }

        internal static string GetTargetAliasName(string sourceName, PropertyMatcherOptions options)
        {
            // will return sourceName if there is no alias
            return options.GetAliasName(sourceName);
        }

        private static List<PropertyInfo> GetFilteredSourcePropertyInfo(Type sourceType, PropertyMatcherOptions options)
        {
            var propertyInfo = ReflectionCache
                .GetPropertyInfo(sourceType, options.Binding)!
                .AsReadOnlyCollection();

            var sourceProps = new List<PropertyInfo>(propertyInfo.Count);

            // Deliberately written without the use of LINQ - benchmarking shows better performance and less memory allocations
            foreach (var propInfo in propertyInfo)
            {
                // With regards to 'options.Filter', apart from a performance benefit, the source properties must be filtered before looking for
                // matches (below) just in case the source contains a property name that is not required (excluded via the Filter) but is mapped
                // to a target property of the same name. Without the pre-filtering, the source selector used in FindMatches() would result in
                // that property name being added twice, resulting in a duplicate key error.
                if (propInfo.CanRead &&
                    !options.IsExcluded(propInfo.Name) &&
                    (options.Filter is null || options.Filter.Invoke(propInfo)))
                {
                    sourceProps.Add(propInfo);
                }
            }

            return sourceProps;
        }

        private static List<PropertyInfo> GetFilteredTargetPropertyInfo(Type targetType, PropertyMatcherOptions options)
        {
            var propertyInfo = ReflectionCache
                .GetPropertyInfo(targetType, options.Binding)!
                .AsReadOnlyCollection();

            var targetProps = new List<PropertyInfo>(propertyInfo.Count);

            // Deliberately written without the use of LINQ - benchmarking shows better performance and less memory allocations
            foreach (var propInfo in propertyInfo)
            {
                if (propInfo.CanWrite)
                {
                    targetProps.Add(propInfo);
                }
            }

            return targetProps;
        }
    }
}