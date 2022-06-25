﻿using AllOverIt.Assertion;
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

            var sourceProps = GetFilteredSourcePropertyInfo(sourceType, options);
            var targetProps = GetFilteredTargetPropertyInfo(targetType, options);

            return sourceProps
                .FindMatches(
                    targetProps,
                    src => GetTargetAliasName(src.Name, options),
                    target => target.Name)
                .AsReadOnlyCollection();
        }

        // Only to be used when property values need to be get/set based on binding options (ie., static methods, never ObjectMapper)
        internal static void MapPropertyValues(Type sourceType, object source, Type targetType, object target, IReadOnlyCollection<PropertyInfo> matches,
            ObjectMapperOptions mapperOptions)
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(target));
            _ = matches.WhenNotNull(nameof(matches));                   // allow empty
            _ = mapperOptions.WhenNotNull(nameof(mapperOptions));

            var sourcePropertyInfo = ReflectionCache
                .GetPropertyInfo(sourceType, mapperOptions.Binding)
                .ToDictionary(prop => prop.Name);

            var targetPropertyInfo = ReflectionCache
                .GetPropertyInfo(targetType, mapperOptions.Binding)
                .ToDictionary(prop => prop.Name);

            foreach (var match in matches)
            {
                var value = sourcePropertyInfo[match.Name].GetValue(source);
                var targetName = GetTargetAliasName(match.Name, mapperOptions);
                var targetValue = mapperOptions.GetConvertedValue(match.Name, value);

                targetPropertyInfo[targetName].SetValue(target, targetValue);
            }
        }

        internal static string GetTargetAliasName(string sourceName, ObjectMapperOptions options)
        {
            return options.GetAliasName(sourceName) ?? sourceName;
        }

        private static IEnumerable<PropertyInfo> GetFilteredSourcePropertyInfo(Type sourceType, ObjectMapperOptions options)
        {
            var propertyInfo = ReflectionCache
                .GetPropertyInfo(sourceType, options.Binding)
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
                    (options.Filter == null || options.Filter.Invoke(propInfo)))
                {
                    sourceProps.Add(propInfo);
                }
            }

            return sourceProps;
        }

        private static IEnumerable<PropertyInfo> GetFilteredTargetPropertyInfo(Type targetType, ObjectMapperOptions options)
        {
            var propertyInfo = ReflectionCache
                .GetPropertyInfo(targetType, options.Binding)
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