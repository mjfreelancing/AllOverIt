using AllOverIt.Assertion;
using AllOverIt.Reflection;
using System.Reflection;

namespace AllOverIt.Mapping
{
    /// <summary>Provides options that control how source properties are matched and copied onto a target instance.</summary>
    public class PropertyMatcherOptions
    {
        private sealed class TargetOptions
        {
            public bool Exclude { get; set; }                           // Efficiently excluded when processing the configuration
            public Func<object?, bool>? ExcludeWhen { get; set; }       // Excluded at runtime based on the source value
            public bool DeepCopy { get; set; }
            public string? Alias { get; set; }
            public object? NullReplacement { get; set; }
            public Func<IObjectMapper, object?, object?>? Converter { get; set; }
        }

        // Source property to target options
        private readonly Dictionary<string, TargetOptions> _sourceTargetOptions = [];

        internal static readonly PropertyMatcherOptions None = new();

        /// <summary>The binding options used to determine how properties on the source object are discovered.</summary>
        public BindingOptions Binding { get; set; } = BindingOptions.Default;

        /// <summary>Use to filter out source properties discovered based on the <see cref="Binding"/> option used.</summary>
        public Func<PropertyInfo, bool>? Filter { get; set; }

        /// <summary>Excludes one or more source properties from object mapping.</summary>
        /// <param name="sourceNames">One or more source property names to be excluded from mapping.</param>
        /// <returns>The same <see cref="PropertyMatcherOptions"/> instance so a fluent syntax can be used.</returns>
        public PropertyMatcherOptions Exclude(params string[] sourceNames)
        {
            _ = sourceNames.WhenNotNull(nameof(sourceNames));

            foreach (var sourceName in sourceNames)
            {
                UpdateTargetOptions(sourceName, targetOptions => targetOptions.Exclude = true);
            }

            return this;
        }

        /// <summary>Excludes a source property from mapping at runtime based on a predicate applied to the property's value.</summary>
        /// <param name="sourceName">The name of the property to exclude when a predicate condition is met.</param>
        /// <param name="predicate">The predicate to apply to the source property value at runtime.</param>
        /// <returns>The same <see cref="PropertyMatcherOptions"/> instance so a fluent syntax can be used.</returns>
        public PropertyMatcherOptions ExcludeWhen(string sourceName, Func<object?, bool> predicate)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));
            _ = predicate.WhenNotNull(nameof(predicate));

            UpdateTargetOptions(sourceName, targetOptions => targetOptions.ExcludeWhen = predicate);

            return this;
        }

        /// <summary>Configures one or more source properties for deep cloning when object mapping. All child object
        /// properties will be cloned.</summary>
        /// <param name="sourceNames">One or more source property names to be configured for deep cloning.</param>
        /// <returns>The same <see cref="PropertyMatcherOptions"/> instance so a fluent syntax can be used.</returns>
        public PropertyMatcherOptions DeepCopy(params string[] sourceNames)
        {
            _ = sourceNames.WhenNotNull(nameof(sourceNames));

            foreach (var sourceName in sourceNames)
            {
                UpdateTargetOptions(sourceName, targetOptions => targetOptions.DeepCopy = true);
            }

            return this;
        }

        /// <summary>Maps a property on the source type to an alias property on the target type.</summary>
        /// <param name="sourceName">The source type property name.</param>
        /// <param name="targetName">The target type property name.</param>
        /// <remarks>There is no validation of the property names provided. An exception will be thrown at runtime if
        /// no matching property name can be found.</remarks>
        /// <returns>The same <see cref="PropertyMatcherOptions"/> instance so a fluent syntax can be used.</returns>
        public PropertyMatcherOptions WithAlias(string sourceName, string targetName)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));
            _ = targetName.WhenNotNullOrEmpty(nameof(targetName));

            UpdateTargetOptions(sourceName, targetOptions => targetOptions.Alias = targetName);

            return this;
        }

        /// <summary>Specifies a value to assign to the target property when the source property is null.</summary>
        /// <param name="sourceName">The source type property name.</param>
        /// <param name="nullReplacement">The value to assign to the target property when the source value is null.</param>
        /// <returns>The same <see cref="PropertyMatcherOptions"/> instance so a fluent syntax can be used.</returns>
        public PropertyMatcherOptions UseWhenNull(string sourceName, object nullReplacement)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));

            UpdateTargetOptions(sourceName, targetOptions => targetOptions.NullReplacement = nullReplacement);

            return this;
        }

        /// <summary>Provides a source to target property value converter. This can be used when there is no implicit
        /// conversion available between the source and target types. The converted value will be assigned to the target
        /// instance.</summary>
        /// <param name="sourceName">The source type property name.</param>
        /// <param name="converter">The source to target value conversion delegate.</param>
        /// <returns>The same <see cref="PropertyMatcherOptions"/> instance so a fluent syntax can be used.</returns>
        public PropertyMatcherOptions WithConversion(string sourceName, Func<IObjectMapper, object?, object?> converter)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));
            _ = converter.WhenNotNull(nameof(converter));

            UpdateTargetOptions(sourceName, targetOptions => targetOptions.Converter = converter);

            return this;
        }

        internal bool IsExcluded(string sourceName)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));

            return _sourceTargetOptions.TryGetValue(sourceName, out var targetOptions) && targetOptions.Exclude;
        }

        internal bool IsExcludedWhen(string sourceName, object? sourceValue)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));

            return _sourceTargetOptions.TryGetValue(sourceName, out var targetOptions) && (targetOptions.ExcludeWhen?.Invoke(sourceValue) ?? false);
        }

        internal bool IsDeepCopy(string sourceName)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));

            return _sourceTargetOptions.TryGetValue(sourceName, out var targetOptions) && targetOptions.DeepCopy;
        }

        internal string GetAliasName(string sourceName)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));

            string? sourceAlias = null;

            if (_sourceTargetOptions.TryGetValue(sourceName, out var targetOptions))
            {
                sourceAlias = targetOptions.Alias;
            }

            return sourceAlias ?? sourceName;
        }

        internal object? GetNullReplacement(string sourceName)
        {
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));

            return _sourceTargetOptions.TryGetValue(sourceName, out var targetOptions)
                ? targetOptions.NullReplacement
                : null;
        }

        internal object? GetConvertedValue(IObjectMapper objectMapper, string sourceName, object? sourceValue)
        {
            _ = objectMapper.WhenNotNull(nameof(objectMapper));
            _ = sourceName.WhenNotNullOrEmpty(nameof(sourceName));

            if (sourceValue is null)
            {
                return null;
            }

            var converter = _sourceTargetOptions.TryGetValue(sourceName, out var targetOptions)
                ? targetOptions.Converter
                : null;

            return converter is not null
                ? converter.Invoke(objectMapper, sourceValue)
                : sourceValue;
        }

        private void UpdateTargetOptions(string sourceName, Action<TargetOptions> optionsAction)
        {
            var hasOptions = _sourceTargetOptions.TryGetValue(sourceName, out var targetOptions);

            if (!hasOptions)
            {
                targetOptions = new TargetOptions();
            }

            var options = targetOptions!;

            optionsAction.Invoke(options);

            if (!hasOptions)
            {
                _sourceTargetOptions.Add(sourceName, options);
            }
        }
    }
}