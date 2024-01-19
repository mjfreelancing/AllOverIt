namespace AllOverIt.Mapping
{
    /// <summary>Represents an object mapper that will copy property values from a source onto a target.</summary>
    public interface IObjectMapper
    {
        /// <summary>Default constructs the target type and copies properties from the provided source type.</summary>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <param name="source">The source instance to copy property values from.</param>
        /// <returns>a new target instance after all source properties have been copied. If the source is null
        /// then null will be returned.</returns>
        TTarget Map<TTarget>(object source) 
            where TTarget : class, new();

        /// <summary>Maps properties from a source object onto a provided target instance.</summary>
        /// <typeparam name="TSource">The source type to copy property values from.</typeparam>
        /// <typeparam name="TTarget">The target type the source object is being mapped onto.</typeparam>
        /// <param name="source">The source object to be mapped onto a target.</param>
        /// <param name="target">The target instance to have property values copied onto.</param>
        /// <returns>The same target instance after all source properties have been copied.</returns>
        TTarget Map<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class;
    }
}