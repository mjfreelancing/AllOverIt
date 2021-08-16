using System;

namespace AllOverIt.Exceptions
{
    public class ProcessException : Exception
    {
        /// <summary>Default constructor.</summary>
        public ProcessException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public ProcessException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ProcessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}