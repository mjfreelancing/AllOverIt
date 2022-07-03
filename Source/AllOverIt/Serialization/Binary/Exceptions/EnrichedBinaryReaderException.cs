using System;

namespace AllOverIt.Serialization.Binary.Exceptions
{
    /// <summary>Represents an error while reading from a binary stream.</summary>
    public class EnrichedBinaryReaderException : Exception
    {
        /// <summary>Default constructor.</summary>
        public EnrichedBinaryReaderException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public EnrichedBinaryReaderException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public EnrichedBinaryReaderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
