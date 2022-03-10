using System;

namespace AllOverIt.Mapping.Extensions
{
    /// <summary>Provides extension methods for setting options when mapping a type object to a target type.</summary>
    public static class ObjectMapperOptionsExtensions
    {
        /// <summary>Excludes one or more source properties from object mapping.</summary>
        /// <param name="mapperOptions">The options instance to update.</param>
        /// <param name="sourceNames">One or more source property names to be excluded from mapping.</param>
        /// <returns>The same <see cref="ObjectMapperOptions"/> instance so a fluent syntax can be used.</returns>
        public static ObjectMapperOptions Exclude(this ObjectMapperOptions mapperOptions, params string[] sourceNames)
        {
            foreach (var sourceName in sourceNames)
            {
                _ = UpdateTargetOptions(mapperOptions, sourceName, targetOptions => targetOptions.Excluded = true);
            }

            return mapperOptions;
        }

        /// <summary>Maps a property on the source type to an alias property on the target type.</summary>
        /// <param name="mapperOptions">The options instance to update.</param>
        /// <param name="sourceName">The source type property name.</param>
        /// <param name="targetName">The target type property name.</param>
        /// <remarks>There is no validation of the property names provided. An exception will be thrown at runtime if
        /// no matching property name can be found.</remarks>
        /// <returns>The same <see cref="ObjectMapperOptions"/> instance so a fluent syntax can be used.</returns>
        public static ObjectMapperOptions WithAlias(this ObjectMapperOptions mapperOptions, string sourceName, string targetName)
        {
            return UpdateTargetOptions(mapperOptions, sourceName, targetOptions => targetOptions.Alias = targetName);
        }

        /// <summary>Provides a source to target property value converter. This can be used when there is no implicit
        /// conversion available between the source and target types.</summary>
        /// <param name="mapperOptions">The options instance to update.</param>
        /// <param name="sourceName">The source type property name.</param>
        /// <param name="converter">The source to target value conversion delegate.</param>
        /// <returns>The same <see cref="ObjectMapperOptions"/> instance so a fluent syntax can be used.</returns>
        public static ObjectMapperOptions WithConversion(this ObjectMapperOptions mapperOptions, string sourceName, Func<object, object> converter)
        {
            return UpdateTargetOptions(mapperOptions, sourceName, targetOptions => targetOptions.Converter = converter);
        }

        internal static bool IsExcluded(this ObjectMapperOptions mapperOptions, string sourceName)
        {
            return mapperOptions.SourceTargetOptions.TryGetValue(sourceName, out var targetOptions) && targetOptions.Excluded;
        }

        internal static string GetAliasName(this ObjectMapperOptions mapperOptions, string sourceName)
        {
            return mapperOptions.SourceTargetOptions.TryGetValue(sourceName, out var targetOptions)
                ? targetOptions.Alias
                : sourceName;
        }

        internal static object GetConvertedValue(this ObjectMapperOptions mapperOptions, string sourceName, object sourceValue)
        {
            var converter = mapperOptions.SourceTargetOptions.TryGetValue(sourceName, out var targetOptions)
                ? targetOptions.Converter
                : null;

            return converter != null
                ? converter.Invoke(sourceValue)
                : sourceValue;
        }

        private static ObjectMapperOptions UpdateTargetOptions(ObjectMapperOptions mapperOptions, string sourceName, Action<ObjectMapperOptions.TargetOptions> optionsAction)
        {
            var hasOptions = mapperOptions.SourceTargetOptions.TryGetValue(sourceName, out var targetOptions);

            if (!hasOptions)
            {
                targetOptions = new ObjectMapperOptions.TargetOptions();
            }

            optionsAction.Invoke(targetOptions);

            if (!hasOptions)
            {
                mapperOptions.SourceTargetOptions.Add(sourceName, targetOptions);
            }

            return mapperOptions;
        }
    }
}