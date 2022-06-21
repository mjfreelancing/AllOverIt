using System;

namespace AllOverIt.Exceptions
{
    /// <summary>Represents an reflection related error.</summary>
    public class ReflectionException : Exception
    {
        /// <summary>Default constructor.</summary>
        public ReflectionException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public ReflectionException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ReflectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}