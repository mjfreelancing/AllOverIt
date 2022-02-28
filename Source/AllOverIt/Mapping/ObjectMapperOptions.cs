using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using AllOverIt.Extensions;
using AllOverIt.Reflection;

namespace AllOverIt.Mapping
{
    //public sealed record ObjectPropertyAlias
    //{
    //    public string SourceName { get; }
    //    public string TargetName { get; }

    //    public ObjectPropertyAlias(string sourceName, string targetName)
    //    {
    //        SourceName = sourceName;
    //        TargetName = targetName;
    //    }
    //}

    /// <summary>Provides options that control how source properties are copied onto a target instance.</summary>
    public class ObjectMapperOptions
    {
        internal readonly IDictionary<string, string> Aliases = new Dictionary<string, string>();

        /// <summary>The binding options used to determine how properties on the source object are discovered.</summary>
        public BindingOptions Binding { get; set; } = BindingOptions.Default;

        /// <summary>Use to filter out source properties discovered based on the <see cref="Binding"/> option used.</summary>
        public Func<PropertyInfo, bool> Filter { get; set; }

        /// <summary>Provides a list of source to target property name aliases.</summary>
        //public IReadOnlyDictionary<string, string> Aliases => PropertyAliases;
    }


    public sealed class TypedObjectMapperOptions<TSource, TTarget> : ObjectMapperOptions
        where TSource : class
        where TTarget : class
    {
        public TypedObjectMapperOptions<TSource, TTarget> WithAlias<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression,
            Expression<Func<TTarget, TProperty>> targetExpression)
        {
            var sourceName = sourceExpression.UnwrapMemberExpression().Member.Name;
            var targetName = targetExpression.UnwrapMemberExpression().Member.Name;

            Aliases.Add(sourceName, targetName);

            return this;
        }
    }
}