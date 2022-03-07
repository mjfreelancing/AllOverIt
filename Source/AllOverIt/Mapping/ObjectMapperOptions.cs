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
        // Source to target property name aliases
        private readonly IDictionary<string, string> _aliases = new Dictionary<string, string>();

        /// <summary>The binding options used to determine how properties on the source object are discovered.</summary>
        public BindingOptions Binding { get; set; } = BindingOptions.Default;

        /// <summary>Use to filter out source properties discovered based on the <see cref="Binding"/> option used.</summary>
        public Func<PropertyInfo, bool> Filter { get; set; }

        /// <summary>Provides an enumerable of all source-to-target defined aliases.</summary>
        public IEnumerable<KeyValuePair<string, string>> Aliases => _aliases;

        /// <summary>Gets the alias name for a specified source property name.</summary>
        /// <param name="sourceName">The source property name.</param>
        /// <returns>The target alias property name.</returns>
        public string GetAlias(string sourceName)
        {
            return _aliases.GetValueOrDefault(sourceName);
        }

        /// <summary>Maps a property on the source object to an alias property on the target object.</summary>
        /// <param name="sourceName">The source object property name.</param>
        /// <param name="targetName">The target object property name.</param>
        /// <remarks>There is no validation of the property names provided. An exception will be thrown at runtime if no matching property name can be found.</remarks>
        public void AddAlias(string sourceName, string targetName)
        {
            _aliases.Add(sourceName, targetName);
        }
    }


    public sealed class TypedObjectMapperOptions<TSource, TTarget> : ObjectMapperOptions
        where TSource : class
        where TTarget : class
    {
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

            AddAlias(sourceName, targetName);

            return this;
        }
    }
}