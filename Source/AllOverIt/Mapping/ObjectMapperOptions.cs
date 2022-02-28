using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using AllOverIt.Extensions;
using AllOverIt.Reflection;

namespace AllOverIt.Mapping
{
    public sealed class ObjectPropertyAlias
    {
        public string SourceName { get; }
        public string TargetName { get; }

        public ObjectPropertyAlias(string sourceName, string targetName)
        {
            SourceName = sourceName;
            TargetName = targetName;
        }
    }

    /// <summary>Provides options that control how source properties are copied onto a target instance.</summary>
    public class ObjectMapperOptions
    {
        // TODO: Update
        internal readonly IList<ObjectPropertyAlias> _aliases = new List<ObjectPropertyAlias>();

        //private readonly IList<(string SourceName, string TargetName)> _aliases = new List<(string, string)>();

        /// <summary>The binding options used to determine how properties on the source object are discovered.</summary>
        public BindingOptions Binding { get; set; } = BindingOptions.Default;

        /// <summary>Use to filter out source properties discovered based on the <see cref="Binding"/> option used.</summary>
        public Func<PropertyInfo, bool> Filter { get; set; }

        /// <summary>Provides a list of source to target property name aliases.</summary>
        public IReadOnlyCollection<ObjectPropertyAlias> Aliases => _aliases.AsReadOnlyCollection();
    }


    public sealed class TypedObjectMapperOptions<TSource, TTarget> : ObjectMapperOptions
        where TSource : class
        where TTarget : class
    {
        //private readonly IList<(string SourceName, string TargetName)> _aliases = new List<(string, string)>();
        
        //internal void AddAlias(string sourceName, string targetName)
        //{
        //    _aliases.Add((sourceName, targetName));
        //}

        public TypedObjectMapperOptions<TSource, TTarget> WithAlias<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression,
            Expression<Func<TTarget, TProperty>> targetExpression)
        {
            var sourceName = sourceExpression.UnwrapMemberExpression().Member.Name;
            var targetName = targetExpression.UnwrapMemberExpression().Member.Name;

            var alias = new ObjectPropertyAlias(sourceName, targetName);
            _aliases.Add(alias);

            return this;
        }
    }




    //public static class ObjectMapperOptionsExtensions
    //{
    //    public static TypedObjectMapperOptions<TSource, TTarget> Alias<TProperty>(this TypedObjectMapperOptions<TSource, TTarget> mapperOptions,
    //        Expression<Func<TSource, TSourceProperty>> sourceExpression, Expression<Func<TTarget, TTargetProperty>> targetExpression)
    //        where TSource : class
    //        where TTarget : class
    //    {
    //        var sourceName = sourceExpression.UnwrapMemberExpression().Member.Name;
    //        var targetName = targetExpression.UnwrapMemberExpression().Member.Name;

    //        mapperOptions.AddAlias(sourceName, targetName);

    //        return mapperOptions;
    //    }
    //}

}