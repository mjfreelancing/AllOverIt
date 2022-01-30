namespace AllOverIt.Serialization.Abstractions
{
    /// <summary>Provides configuration options to apply to a <see cref="IJsonSerializer"/>.</summary>
    public sealed class JsonSerializerConfiguration
    {
        /// <summary>If True, property names are treated as case-insensitive. If null, the current setting is not changed.</summary>
        /// <remarks>The default is False. Newtonsoft is case insensitive by default and requires a custom converter to support case sensitivity.</remarks>
        public bool? CaseSensitive { get; set; }

        /// <summary>If True, use the Camel case naming convention in preference to Pascal casing. If null, the current setting is not changed.</summary>
        /// <remarks>The default is False. Pascal case naming convention is the default.</remarks>
        public bool? UseCamelCase { get; set; }

        /// <summary>If True, the serializer will support the serialization and deserialization of enriched enum types (available in the AllOverIt package).</summary>
        public bool? SupportEnrichedEnums { get; set; }
    }
}