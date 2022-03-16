using AllOverIt.Extensions;
using System;
using System.Linq.Expressions;
using AllOverIt.Assertion;

namespace AllOverIt.Mapping
{
    /// <summary>Provides options that control how source properties are copied onto a target instance.</summary>
    /// <typeparam name="TSource">The source object type.</typeparam>
    /// <typeparam name="TTarget">The target object type.</typeparam>
    public sealed class TypedObjectMapperOptions<TSource, TTarget> : ObjectMapperOptions
        where TSource : class
        where TTarget : class
    {
        private ObjectMapperOptions Options => this;

        /// <summary>Excludes one or more source properties from object mapping.</summary>
        /// <typeparam name="TProperty">The source property type.</typeparam>
        /// <param name="sourceExpression">An expression to specify the source property being excluded.</param>
        /// <returns>The same <see cref="TypedObjectMapperOptions{TSource, TTarget}"/> instance so a fluent syntax can be used.</returns>
        public TypedObjectMapperOptions<TSource, TTarget> Exclude<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression)
        {
            _ = sourceExpression.WhenNotNull(nameof(sourceExpression));

            // TODO: Validate the expressions are only one property deep
            var sourceName = sourceExpression.UnwrapMemberExpression().Member.Name;

            Options.Exclude(sourceName);

            return this;
        }

        /// <summary>Maps a property on the source object to an alias property on the target object.</summary>
        /// <typeparam name="TProperty">The source property type.</typeparam>
        /// <param name="sourceExpression">An expression to specify the property on the source object.</param>
        /// <param name="targetExpression">An expression to specify the property on the target object.</param>
        /// <returns>The same <see cref="TypedObjectMapperOptions{TSource, TTarget}"/> instance so a fluent syntax can be used.</returns>
        public TypedObjectMapperOptions<TSource, TTarget> WithAlias<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression,
            Expression<Func<TTarget, TProperty>> targetExpression)
        {
            _ = sourceExpression.WhenNotNull(nameof(sourceExpression));
            _ = targetExpression.WhenNotNull(nameof(targetExpression));

            // TODO: Validate the expressions are only one property deep
            var sourceName = sourceExpression.UnwrapMemberExpression().Member.Name;
            var targetName = targetExpression.UnwrapMemberExpression().Member.Name;

            Options.WithAlias(sourceName, targetName);

            return this;
        }

        /// <summary>Provides a source to target property value converter. This can be used when there is no implicit
        /// conversion available between the source and target types.</summary>
        /// <typeparam name="TProperty">The source property type.</typeparam>
        /// <param name="sourceExpression">An expression to specify the property on the source object.</param>
        /// <param name="converter">The source to target value conversion delegate.</param>
        /// <returns>The same <see cref="TypedObjectMapperOptions{TSource, TTarget}"/> instance so a fluent syntax can be used.</returns>
        public TypedObjectMapperOptions<TSource, TTarget> WithConversion<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression,
            Func<TProperty, object> converter)
        {
            _ = sourceExpression.WhenNotNull(nameof(sourceExpression));
            _ = converter.WhenNotNull(nameof(converter));

            // TODO: Validate the expressions are only one property deep
            var sourceName = sourceExpression.UnwrapMemberExpression().Member.Name;

            Options.WithConversion(sourceName, source => converter.Invoke((TProperty) source));

            return this;
        }
    }
}