namespace AllOverIt.Formatters.Objects
{
    /// <summary>Provides options that define root level value key names for objects that cannot be serialized due
    /// to the lack of properties.</summary>
    public sealed class ObjectPropertyRootValueOptions
    {
        /// <summary>Specifies the key name to use when serializing a root level value that is a scalar type.</summary>
        public string ScalarKeyName { get; set; } = "_";

        /// <summary>Specifies the key name to use when serializing a root level value that is an array/collection type.</summary>
        public string ArrayKeyName { get; set; } = "[]";
    }
}