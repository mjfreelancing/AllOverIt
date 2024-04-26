namespace AllOverIt.Mapping.Exceptions
{
    /// <summary>Represents an error that occurred while object mapping.</summary>
    public sealed class ObjectMapperException : Exception
    {
        /// <summary>Default constructor.</summary>
        public ObjectMapperException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public ObjectMapperException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ObjectMapperException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}