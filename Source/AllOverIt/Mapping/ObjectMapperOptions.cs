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
        internal readonly IDictionary<string, string> Aliases = new Dictionary<string, string>();

        /// <summary>The binding options used to determine how properties on the source object are discovered.</summary>
        public BindingOptions Binding { get; set; } = BindingOptions.Default;

        /// <summary>Use to filter out source properties discovered based on the <see cref="Binding"/> option used.</summary>
        public Func<PropertyInfo, bool> Filter { get; set; }
    }


    public sealed class TypedObjectMapperOptions<TSource, TTarget> : ObjectMapperOptions
        where TSource : class
        where TTarget : class
    {
        public TypedObjectMapperOptions<TSource, TTarget> WithAlias<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression,
            Expression<Func<TTarget, TProperty>> targetExpression)
        {
            // TODO: Validate the expressions are only one property deep
            var sourceName = sourceExpression.UnwrapMemberExpression().Member.Name;
            var targetName = targetExpression.UnwrapMemberExpression().Member.Name;

            Aliases.Add(sourceName, targetName);

            return this;
        }
    }
}