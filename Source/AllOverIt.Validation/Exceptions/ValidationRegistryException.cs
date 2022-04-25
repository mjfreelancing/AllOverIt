using System;

namespace AllOverIt.Validation.Exceptions
{
    /// <summary>Represents an error that occurred while registering a validator.</summary>
    public class ValidationRegistryException : Exception
    {
        /// <summary>Default constructor.</summary>
        public ValidationRegistryException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public ValidationRegistryException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ValidationRegistryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}