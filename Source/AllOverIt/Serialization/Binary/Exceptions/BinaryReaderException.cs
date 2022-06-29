using System;

namespace AllOverIt.Serialization.Binary.Exceptions
{
    /// <summary>Represents an error while reading from a binary stream.</summary>
    public class BinaryReaderException : Exception
    {
        /// <summary>Default constructor.</summary>
        public BinaryReaderException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public BinaryReaderException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public BinaryReaderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
