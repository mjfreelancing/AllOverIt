namespace AllOverIt.Patterns.Enumeration.Exceptions
{
    /// <summary>Represents an error that occurred while attempting to interpret a name or value as a <see cref="EnrichedEnum{TEnum}"/>.</summary>
    public sealed class EnrichedEnumException : Exception
    {
        /// <summary>Default constructor.</summary>
        public EnrichedEnumException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public EnrichedEnumException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public EnrichedEnumException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}