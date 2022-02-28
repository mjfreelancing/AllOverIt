using System;

namespace AllOverIt.Mapping
{
    /// <summary>Represents an object mapper that will copy property values from a source onto a target.</summary>
    public interface IObjectMapper
    {
        /// <summary>Allows source to target mapping configuration to be initialized in advance. If not performed in
        /// advance, the same configuration will be performed the first time a mapping operation is performed.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <param name="configure">The action that allows the mapper options to be configured. Configuration is cached
        /// based on the source type, target type, and binding options used.</param>
        void Configure<TSource, TTarget>(Action<ObjectMapperOptions> configure)
            where TSource : class
            where TTarget : class;

        /// <summary>Default constructs the target type and copies properties from the provided source type.</summary>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <param name="source">The source instance to copy property values from.</param>
        /// <param name="configure">The action that allows the mapper options to be configured. Configuration is cached
        /// based on the source type, target type, and binding options used.</param>
        /// <returns>The same target instance after all source properties have been copied.</returns>
        TTarget Map<TTarget>(object source, Action<ObjectMapperOptions> configure = default) 
            where TTarget : class, new();

        /// <summary>Maps properties from a source object onto a provided target instance.</summary>
        /// <typeparam name="TSource">The source type to copy property values from.</typeparam>
        /// <typeparam name="TTarget">The target type the source object is being mapped onto.</typeparam>
        /// <param name="source">The source object to be mapped onto a target.</param>
        /// <param name="target">The target instance to have property values copied onto.</param>
        /// <param name="configure">The action that allows the mapper options to be configured. Configuration is cached
        /// based on the source type, target type, and binding options used.</param>
        /// <returns>The same target instance after all source properties have been copied.</returns>
        TTarget Map<TSource, TTarget>(TSource source, TTarget target, Action<ObjectMapperOptions> configure = default)
            where TSource : class
            where TTarget : class;
    }
}