using AllOverIt.Assertion;
using AllOverIt.Helpers.PropertyNavigation;
using AllOverIt.Helpers.PropertyNavigation.Extensions;
using AllOverIt.Mapping.Exceptions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Mapping
{
    /// <summary>Provides options that control how source properties are copied onto a target instance.</summary>
    /// <typeparam name="TSource">The source object type.</typeparam>
    /// <typeparam name="TTarget">The target object type.</typeparam>
    public sealed class TypedObjectMapperOptions<TSource, TTarget> : ObjectMapperOptions
    {
        // source type, target type, factory (ampper, source, target)
        private readonly Action<Type, Type, Func<IObjectMapper, object, object>> _sourceTargetFactoryRegistration;

        /// <summary>Constructor.</summary>
        /// <param name="mapper">The associated object mapper.</param>
        /// <param name="sourceTargetFactoryRegistration">Used to register a target type factory with the object mapper for a provided
        /// source and target type combination.</param>
        internal TypedObjectMapperOptions(IObjectMapper mapper, Action<Type, Type, Func<IObjectMapper, object, object>> sourceTargetFactoryRegistration)
            : base(mapper)
        {
            _sourceTargetFactoryRegistration = sourceTargetFactoryRegistration.WhenNotNull(nameof(sourceTargetFactoryRegistration));
        }

        /// <summary>Excludes a source property from object mapping.</summary>
        /// <typeparam name="TProperty">The source property type.</typeparam>
        /// <param name="sourceExpression">An expression to specify the source property being excluded.</param>
        /// <returns>The same <see cref="TypedObjectMapperOptions{TSource, TTarget}"/> instance so a fluent syntax can be used.</returns>
        public TypedObjectMapperOptions<TSource, TTarget> Exclude<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression)
        {
            _ = sourceExpression.WhenNotNull(nameof(sourceExpression));

            var sourceName = GetPropertyName(sourceExpression);

            Exclude(sourceName);

            return this;
        }

        /// <summary>Configures a source property for deep copying when object mapping.</summary>
        /// <typeparam name="TProperty">The source property type.</typeparam>
        /// <param name="sourceExpression">An expression to specify the source property to be deep cloned.</param>
        /// <returns>The same <see cref="TypedObjectMapperOptions{TSource, TTarget}"/> instance so a fluent syntax can be used.</returns>
        public TypedObjectMapperOptions<TSource, TTarget> DeepCopy<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression)
        {
            _ = sourceExpression.WhenNotNull(nameof(sourceExpression));

            var sourceName = GetPropertyName(sourceExpression);

            DeepCopy(sourceName);

            return this;
        }

        /// <summary>Maps a property on the source object to an alias property on the target object.</summary>
        /// <typeparam name="TSourceProperty">The source property type.</typeparam>
        /// <typeparam name="TTargetProperty">The target property type.</typeparam>
        /// <param name="sourceExpression">An expression to specify the property on the source object.</param>
        /// <param name="targetExpression">An expression to specify the property on the target object.</param>
        /// <returns>The same <see cref="TypedObjectMapperOptions{TSource, TTarget}"/> instance so a fluent syntax can be used.</returns>
        public TypedObjectMapperOptions<TSource, TTarget> WithAlias<TSourceProperty, TTargetProperty>(Expression<Func<TSource, TSourceProperty>> sourceExpression,
            Expression<Func<TTarget, TTargetProperty>> targetExpression)
        {
            _ = sourceExpression.WhenNotNull(nameof(sourceExpression));
            _ = targetExpression.WhenNotNull(nameof(targetExpression));

            var sourceName = GetPropertyName(sourceExpression);
            var targetName = GetPropertyName(targetExpression);

            WithAlias(sourceName, targetName);

            return this;
        }

        /// <summary>Provides a source to target property value converter. This can be used when there is no implicit
        /// conversion available between the source and target types.</summary>
        /// <typeparam name="TProperty">The source property type.</typeparam>
        /// <param name="sourceExpression">An expression to specify the property on the source object.</param>
        /// <param name="converter">The source to target value conversion delegate.</param>
        /// <returns>The same <see cref="TypedObjectMapperOptions{TSource, TTarget}"/> instance so a fluent syntax can be used.</returns>
        public TypedObjectMapperOptions<TSource, TTarget> WithConversion<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression,
            Func<IObjectMapper, TProperty, object> converter)
        {
            _ = sourceExpression.WhenNotNull(nameof(sourceExpression));
            _ = converter.WhenNotNull(nameof(converter));

            var sourceName = GetPropertyName(sourceExpression);

            WithConversion(sourceName, (mapper, source) => converter.Invoke(mapper, (TProperty) source));

            return this;
        }

        /// <summary>Provides the option to specify a factory method to create the required target type from a given source.</summary>
        /// <param name="targetFactory">The factory that will create the required target type.</param>
        /// <returns>The same <see cref="TypedObjectMapperOptions{TSource, TTarget}"/> instance so a fluent syntax can be used.</returns>
        public TypedObjectMapperOptions<TSource, TTarget> ConstructUsing(Func<IObjectMapper, TSource, TTarget> targetFactory)
        {
            _ = targetFactory.WhenNotNull(nameof(targetFactory));

            _sourceTargetFactoryRegistration.Invoke(typeof(TSource), typeof(TTarget), (mapper, source) => targetFactory.Invoke(mapper, (TSource) source));

            return this;
        }

        private static string GetPropertyName<TType, TProperty>(Expression<Func<TType, TProperty>> sourceExpression)
        {
            var propertyNodes = PropertyNavigator
                .For<TType>()
                .Navigate(sourceExpression);

            if (propertyNodes.Nodes.Count > 1)
            {
                throw new ObjectMapperException($"ObjectMapper do not support nested mappings ({sourceExpression}).");
            }

            return propertyNodes.Nodes.Single().Name();
        }
    }
}