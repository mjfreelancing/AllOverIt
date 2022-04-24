namespace AllOverIt.Serialization.NewtonsoftJson.Converters
{
    /// <summary>Provides options that control how the <see cref="NestedDictionaryConverter"/> behaves during serialization and deserialization.</summary>
    public sealed class NestedDictionaryConverterOptions
    {
        /// <summary>Indicates if property name casing will be strict (exactly as declared), or if they should all be treated as lower-camel case (the default).</summary>
        public bool StrictPropertyNames { get; init; }
    }
}