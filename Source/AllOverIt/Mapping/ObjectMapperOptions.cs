using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using AllOverIt.Extensions;
using AllOverIt.Reflection;

namespace AllOverIt.Mapping
{
    /// <summary>Provides options that control how source properties are copied onto a target instance.</summary>
    public class ObjectMapperOptions
    {
        private class TargetOptions
        {
            public bool Excluded { get; set; }
            public string Alias { get; set; }
            public Func<object, object> Converter { get; set; }
        }

        // Source property to target options
        private readonly IDictionary<string, TargetOptions> _targetOptions = new Dictionary<string, TargetOptions>();

        /// <summary>The binding options used to determine how properties on the source object are discovered.</summary>
        public BindingOptions Binding { get; set; } = BindingOptions.Default;

        /// <summary>Use to filter out source properties discovered based on the <see cref="Binding"/> option used.</summary>
        public Func<PropertyInfo, bool> Filter { get; set; }

        public bool IsExcluded(string sourceName)
        {
            return _targetOptions.TryGetValue(sourceName, out var targetOptions) && targetOptions.Excluded;
        }

        /// <summary>Gets the alias name for a specified source property name.</summary>
        /// <param name="sourceName">The source property name.</param>
        /// <returns>The target alias property name.</returns>
        public string GetAliasName(string sourceName)
        {
            return _targetOptions.TryGetValue(sourceName, out var targetOptions)
                ? targetOptions.Alias
                : sourceName;
        }

        public object GetConvertedValue(string sourceName, object sourceValue)
        {
            var converter = _targetOptions.TryGetValue(sourceName, out var targetOptions)
                ? targetOptions.Converter
                : null;

            return converter != null
                ? converter.Invoke(sourceValue)
                : sourceValue;
        }

        public ObjectMapperOptions Exclude(params string[] sourceNames)
        {
            foreach (var sourceName in sourceNames)
            {
                if (_targetOptions.TryGetValue(sourceName, out var targetOptions))
                {
                    targetOptions.Excluded = true;
                }
                else
                {
                    targetOptions = new TargetOptions
                    {
                        Excluded = true
                    };

                    _targetOptions.Add(sourceName, targetOptions);
                }
            }

            return this;
        }

        /// <summary>Maps a property on the source object to an alias property on the target object.</summary>
        /// <param name="sourceName">The source object property name.</param>
        /// <param name="targetName">The target object property name.</param>
        /// <remarks>There is no validation of the property names provided. An exception will be thrown at runtime if no matching property name can be found.</remarks>
        /// <returns>The same <see cref="ObjectMapperOptions"/> instance so a fluent syntax can be used.</returns>
        public ObjectMapperOptions WithAlias(string sourceName, string targetName)
        {
            if (_targetOptions.TryGetValue(sourceName, out var targetOptions))
            {
                targetOptions.Alias = targetName;
            }
            else
            {
                targetOptions = new TargetOptions
                {
                    Alias = targetName
                };

                _targetOptions.Add(sourceName, targetOptions);
            }

            return this;
        }

        public ObjectMapperOptions WithConversion(string sourceName, Func<object, object> converter)
        {
            if (_targetOptions.TryGetValue(sourceName, out var targetOptions))
            {
                targetOptions.Converter = converter;
            }
            else
            {
                targetOptions = new TargetOptions
                {
                    Converter = converter
                };

                _targetOptions.Add(sourceName, targetOptions);
            }

            return this;
        }
    }


    public sealed class TypedObjectMapperOptions<TSource, TTarget> : ObjectMapperOptions
        where TSource : class
        where TTarget : class
    {
        public TypedObjectMapperOptions<TSource, TTarget> Exclude<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression)
        {
            // TODO: Validate the expressions are only one property deep
            var sourceName = sourceExpression.UnwrapMemberExpression().Member.Name;

            Exclude(sourceName);

            return this;
        }

        /// <summary>Maps a property on the source object to an alias property on the target object.</summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="sourceExpression">An expression for the property on the source object.</param>
        /// <param name="targetExpression">An expression for the property on the target object.</param>
        /// <returns>The same <see cref="TypedObjectMapperOptions{TSource, TTarget}"/> instance so a fluent syntax can be used.</returns>
        public TypedObjectMapperOptions<TSource, TTarget> WithAlias<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression,
            Expression<Func<TTarget, TProperty>> targetExpression)
        {
            // TODO: Validate the expressions are only one property deep
            var sourceName = sourceExpression.UnwrapMemberExpression().Member.Name;
            var targetName = targetExpression.UnwrapMemberExpression().Member.Name;

            WithAlias(sourceName, targetName);

            return this;
        }

        public TypedObjectMapperOptions<TSource, TTarget> WithConversion<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression,
            Func<TProperty, object> converter)
        {
            // TODO: Validate the expressions are only one property deep
            var sourceName = sourceExpression.UnwrapMemberExpression().Member.Name;

            WithConversion(sourceName, source => converter.Invoke((TProperty) source));

            return this;
        }
    }
}