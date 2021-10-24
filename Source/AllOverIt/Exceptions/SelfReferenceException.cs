using System;

namespace AllOverIt.Exceptions
{
    /// <summary>Represents on object self-reference error.</summary>
    public class SelfReferenceException : Exception
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
