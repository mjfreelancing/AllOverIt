using System;

namespace AllOverIt.Formatters.Objects.Exceptions
{
    /// <summary>Represents an object self-reference error.</summary>
    public sealed class SelfReferenceException : Exception
    {
        /// <summary>Default constructor.</summary>
        public SelfReferenceException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public SelfReferenceException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SelfReferenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
