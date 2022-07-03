using System;

namespace AllOverIt.Serialization.Binary.Exceptions
{
    /// <summary>Represents an error while writing to a binary stream.</summary>
    public class EnrichedBinaryWriterException : Exception
    {
        /// <summary>Default constructor.</summary>
        public EnrichedBinaryWriterException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public EnrichedBinaryWriterException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public EnrichedBinaryWriterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
