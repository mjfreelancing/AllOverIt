namespace AllOverIt.Formatters.Objects.Exceptions
{
    /// <summary>Represents an error raised by an <see cref="ObjectPropertyFilter"/>.</summary>
    public sealed class ObjectPropertyFilterException : Exception
    {
        /// <summary>Default constructor.</summary>
        public ObjectPropertyFilterException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public ObjectPropertyFilterException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ObjectPropertyFilterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}