namespace AllOverIt.Mapping
{
    /// <summary>Provides global operations for all object mapping.</summary>
    public interface IObjectMapperOptions
    {
        /// <summary>
        /// <para>The default mapping behaviour of collections when the source is <see langword="null"/> is to create an empty array, list, or
        /// dictionary. When <see langword="True"/>, this option changes that behaviour so a <see langword="null"/> source value is mapped as
        /// a <see langword="null"/> target value.</para>
        /// <para>If the target collection cannot be assigned an array, list, or dictionary, then the mapper should be configured
        /// to construct, or convert, the source value to the required type.</para>
        /// </summary>
        bool AllowNullCollections { get; set; }

        /// <summary>Pre-registers a factory for the specified <typeparamref name="TType"/>. The mapper will automatically generate
        /// a factory that will attempt to construct the type using its default constructor.</summary>
        /// <typeparam name="TType">The type being registered.</typeparam>
        /// <remarks>Registering a type is completely optional. If performed, however, it will provide a performance improvement
        /// when needing to create the first instance.</remarks>
        void Register<TType>();

        /// <summary>Registers a factory for the specified <typeparamref name="TType"/>. This can be useful for scenarios where you
        /// need to provide defaults to one or more properties, or the type creation needs to be delegated to an external factory.</summary>
        /// <typeparam name="TType">The type being registered.</typeparam>
        /// <param name="factory">The factory method to invoke when a new <typeparamref name="TType"/> instance is required.</param>
        /// <remarks>Registering a type is completely optional. If performed, however, it will provide a performance improvement
        /// when needing to create the first instance.</remarks>
        void Register<TType>(Func<object> factory);
    }
}