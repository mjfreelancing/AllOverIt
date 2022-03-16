using System;
using System.Collections.Generic;
using System.Reflection;
using AllOverIt.Assertion;
using AllOverIt.Reflection;

namespace AllOverIt.Mapping
{
    /// <summary>Provides options that control how source properties are copied onto a target instance.</summary>
    public class ObjectMapperOptions
    {
        private sealed class TargetOptions
        {
            public bool Excluded { get; set; }
            public string Alias { get; set; }
            public Func<object, object> Converter { get; set; }
        }

        // Source property to target options - updated via extension methods
        private readonly IDictionary<string, TargetOptions> _sourceTargetOptions = new Dictionary<string, TargetOptions>();

        /// <summary>The binding options used to determine how properties on the source object are discovered.</summary>
        public BindingOptions Binding { get; set; } = BindingOptions.Default;

        /// <summary>Use to filter out source properties discovered based on the <see cref="Binding"/> option used.</summary>
        public Func<PropertyInfo, bool> Filter { get; set; }

        /// <summary>Excludes one or more source properties from object mapping.</summary>
        /// <param name="sourceNames">One or more source property names to be excluded from mapping.</param>
        /// <returns>The same <see cref="ObjectMapperOptions"/> instance so a fluent syntax can be used.</returns>
        public ObjectMapperOptions Exclude(params string[] sourceNames)
        {
            _ = sourceNames.WhenNotNull(nameof(sourceNames));

            foreach (var sourceName in sourceNames)
            {
                UpdateTargetOptions(sourceName, targetOptions => targetOptions.Excluded = true);
            }

            return this;
        }

        /// <summary>Maps a property on the source type to an alias property on the target type.</summary>
        /// <param name="sourceName">The source type property name.</param>
        /// <param name="targetName">The target type property name.</param>
        /// <remarks>There is no validation of the property names provided. An exception will be thrown at runtime if
        /// no matching property name can be found.</remarks>
        /// <returns>The same <see cref="ObjectMapperOptions"/> instance so a fluent syntax can be used.</returns>
        public ObjectMapperOptions WithAlias(string sourceName, string targetName)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));
            _ = targetName.WhenNotNullOrEmpty(nameof(targetName));

            UpdateTargetOptions(sourceName, targetOptions => targetOptions.Alias = targetName);

            return this;
        }

        /// <summary>Provides a source to target property value converter. This can be used when there is no implicit
        /// conversion available between the source and target types.</summary>
        /// <param name="sourceName">The source type property name.</param>
        /// <param name="converter">The source to target value conversion delegate.</param>
        /// <returns>The same <see cref="ObjectMapperOptions"/> instance so a fluent syntax can be used.</returns>
        public ObjectMapperOptions WithConversion(string sourceName, Func<object, object> converter)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));
            _ = converter.WhenNotNull(nameof(converter));

            UpdateTargetOptions( sourceName, targetOptions => targetOptions.Converter = converter);

            return this;
        }

        internal bool IsExcluded(string sourceName)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));

            return _sourceTargetOptions.TryGetValue(sourceName, out var targetOptions) && targetOptions.Excluded;
        }

        internal string GetAliasName(string sourceName)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));

            return _sourceTargetOptions.TryGetValue(sourceName, out var targetOptions)
                ? targetOptions.Alias
                : sourceName;
        }

        internal object GetConvertedValue(string sourceName, object sourceValue)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));

            var converter = _sourceTargetOptions.TryGetValue(sourceName, out var targetOptions)
                ? targetOptions.Converter
                : null;

            return converter != null
                ? converter.Invoke(sourceValue)
                : sourceValue;
        }

        private void UpdateTargetOptions(string sourceName, Action<ObjectMapperOptions.TargetOptions> optionsAction)
        {
            var hasOptions = _sourceTargetOptions.TryGetValue(sourceName, out var targetOptions);

            if (!hasOptions)
            {
                targetOptions = new TargetOptions();
            }

            optionsAction.Invoke(targetOptions);

            if (!hasOptions)
            {
                _sourceTargetOptions.Add(sourceName, targetOptions);
            }
        }
    }
}