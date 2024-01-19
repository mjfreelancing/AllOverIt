using System;

namespace AllOverIt.Pipes.Exceptions
{
    /// <summary>Represents an error that occurs while using an anonymous pipe.</summary>
    public class PipeException : Exception
    {
        /// <summary>Default constructor.</summary>
        public PipeException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public PipeException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PipeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}