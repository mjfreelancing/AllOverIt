namespace AllOverIt.Mapping
{
    /// <summary>/// Provides global operations for all object mapping.</summary>
    public sealed class ObjectMapperOptions
    {
        /// <summary>
        /// <para>The default mapping behaviour of collections when the source is null is to create an empty array, list, or
        /// dictionary. This option changes that behaviour so a null source value is mapped as a null target value.</para>
        /// <para>If the target collection cannot be assigned an array, list, or dictionary then the mapper should be configured
        /// to construct or convert the source value to the required type.</para>
        /// </summary>
        public bool AllowNullCollections { get; set; }
    }
}