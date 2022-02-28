using AllOverIt.Assertion;
using AllOverIt.Reflection;

namespace AllOverIt.Mapping.Extensions
{
    /// <summary>Provides extension methods for mapping a source object to a target type.</summary>
    public static class ObjectExtensions
    {
        /// <summary>Maps properties from a source object onto a default constructed target type.</summary>
        /// <typeparam name="TTarget">The target type the source object is being mapped onto.</typeparam>
        /// <param name="source">The source object to be mapped onto a target.</param>
        /// <param name="options">Provides options that control how source properties are copied onto a target instance.</param>
        /// <returns>A new instance of the target type with matching properties copied from the provided source object.</returns>
        public static TTarget MapTo<TTarget>(this object source, ObjectMapperOptions options)
            where TTarget : class, new()
        {
            _ = source.WhenNotNull(nameof(source));
            _ = options.WhenNotNull(nameof(options));

            var target = new TTarget();
            return MapSourceToTarget(source, target, options);
        }

        /// <summary>Maps properties from a source object onto a default constructed target type.</summary>
        /// <typeparam name="TTarget">The target type the source object is being mapped onto.</typeparam>
        /// <param name="source">The source object to be mapped onto a target.</param>
        /// <param name="bindingOptions">The binding options used to determine how properties on the source object are discovered.</param>
        /// <returns>A new instance of the target type with matching properties copied from the provided source object.</returns>
        public static TTarget MapTo<TTarget>(this object source, BindingOptions bindingOptions = BindingOptions.Default)
            where TTarget : class, new()
        {
            _ = source.WhenNotNull(nameof(source));

            var target = new TTarget();
            return MapSourceToTarget(source, target, bindingOptions);
        }

        /// <summary>Maps properties from a source object onto a provided target instance.</summary>
        /// <typeparam name="TSource">The source type to copy property values from.</typeparam>
        /// <typeparam name="TTarget">The target type the source object is being mapped onto.</typeparam>
        /// <param name="source">The source object to be mapped onto a target.</param>
        /// <param name="target">The target instance to have property values copied onto.</param>
        /// <param name="options">Provides options that control how source properties are copied onto a target instance.</param>
        /// <returns>The same target instance after all source properties have been copied.</returns>
        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target, ObjectMapperOptions options)
            where TSource : class
            where TTarget : class
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(target));
            _ = options.WhenNotNull(nameof(options));

            return MapSourceToTarget(source, target, options);
        }

        /// <summary>Maps properties from a source object onto a provided target instance.</summary>
        /// <typeparam name="TSource">The source type to copy property values from.</typeparam>
        /// <typeparam name="TTarget">The target type the source object is being mapped onto.</typeparam>
        /// <param name="source">The source object to be mapped onto a target.</param>
        /// <param name="target">The target instance to have property values copied onto.</param>
        /// <param name="bindingOptions">The binding options used to determine how properties on the source object are discovered.</param>
        /// <returns>The same target instance after all source properties have been copied.</returns>
        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target, BindingOptions bindingOptions = BindingOptions.Default)
            where TSource : class
            where TTarget : class
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(target));

            return MapSourceToTarget(source, target, bindingOptions);
        }

        private static TTarget MapSourceToTarget<TTarget>(object source, TTarget target, ObjectMapperOptions options)
        {
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);

            var matches = ObjectMapperHelper.GetMappableProperties(sourceType, targetType, options.Binding);

            ObjectMapperHelper.MapPropertyValues(sourceType, source, targetType, target, matches, options);

            return target;
        }

        private static TTarget MapSourceToTarget<TTarget>(object source, TTarget target, BindingOptions bindingOptions)
        {
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);

            var matches = ObjectMapperHelper.GetMappableProperties(sourceType, targetType, bindingOptions);

            ObjectMapperHelper.MapPropertyValues(sourceType, source, targetType, target, matches, bindingOptions);

            return target;
        }
    }
}