using AllOverIt.Extensions;
using AllOverIt.Reflection;

namespace AllOverIt.Mapping.Extensions
{
    /// <summary>Provides extension methods for mapping a source object to a target type.</summary>
    public static class ObjectExtensions
    {
        /// <summary>Maps properties from a source object onto a default constructed target type.</summary>
        /// <typeparam name="TTarget">The target type the source object is being mapped onto.</typeparam>
        /// <param name="source">The source object to be mapped onto a target.</param>
        /// <param name="bindingOptions">The binding options used to determine how properties on the source object are discovered.</param>
        /// <returns>A new instance of the target type with matching properties copied from the provided source object.</returns>
        public static TTarget MapTo<TTarget>(this object source, BindingOptions bindingOptions = BindingOptions.Default) where TTarget : new()
        {
            var target = new TTarget();
            return MapSourceToTarget(source, target, bindingOptions);
        }

        /// <summary>Maps properties from a source object onto a provided target instance.</summary>
        /// <typeparam name="TSource">The source type to copy property values from.</typeparam>
        /// <typeparam name="TTarget">The target type the source object is being mapped onto.</typeparam>
        /// <param name="source">The source object to be mapped onto a target.</param>
        /// <param name="target">The target instance to have property values copied onto.</param>
        /// <param name="bindingOptions">The binding options used to determine how properties on the source object are discovered.</param>
        /// <returns>The same target instance after all source properties have been copied.</returns>
        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target, BindingOptions bindingOptions = BindingOptions.Default)
        {
            return MapSourceToTarget(source, target, bindingOptions);
        }

        private static TTarget MapSourceToTarget<TTarget>(object source, TTarget target, BindingOptions bindingOptions)
        {
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);

            var matching = ObjectMapperHelper.GetMappableProperties(sourceType, targetType, bindingOptions);

            foreach (var match in matching)
            {
                var value = source.GetPropertyValue(sourceType, match.Name, bindingOptions);
                target.SetPropertyValue(targetType, match.Name, value, bindingOptions);
            }

            return target;
        }
    }
}