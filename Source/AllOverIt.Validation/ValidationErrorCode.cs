namespace AllOverIt.Validation
{
    /// <summary>Custom error codes that are used with the provided built-in rule extensions.</summary>
    public enum ValidationErrorCode
    {
        /// <summary>The property value is required. Validation fails if the property value is null, an empty string, whitespace,
        /// an empty collection or the default value of the type.</summary>
        /// <remarks>The meaning of this error code is the same as <see cref="NotEmpty"/> but the code and reported message more clearly
        /// indicates the reason for the failure.</remarks>
        Required,

        /// <summary>The property value cannot be empty. Validation fails if the property value is null, an empty string, whitespace,
        /// an empty collection or the default value of the type.</summary>
        /// <remarks>The meaning of this error code is the same as <see cref="Required"/> but the code and reported message more clearly
        /// indicates the reason for the failure.</remarks>
        NotEmpty,

        /// <summary>The property value is out of range.</summary>
        OutOfRange,

        /// <summary>The property value is not unique.</summary>
        Duplicate
    }
}