namespace AllOverIt.Serialization.Json.Abstractions.Exceptions
{
    /// <summary>Represents an error thrown by a concrete instance of <see cref="JsonHelperBase"/>.</summary>
    public sealed class JsonHelperException : Exception
    {
        /// <summary>Default constructor.</summary>
        public JsonHelperException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public JsonHelperException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public JsonHelperException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}