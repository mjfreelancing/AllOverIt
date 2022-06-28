using System;

namespace AllOverIt.Serialization.Binary.Exceptions
{
    /// <summary>Represents on object self-reference error.</summary>
    public class BinaryWriterException : Exception
    {
        /// <summary>Default constructor.</summary>
        public BinaryWriterException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public BinaryWriterException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public BinaryWriterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
