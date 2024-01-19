using System;
using System.IO;
using AllOverIt.Serialization.Binary.Readers;

namespace AllOverIt.Serialization.Binary.Exceptions
{
    /// <summary>Represents an error while reading from a <see cref="BinaryReader"/> or <see cref="EnrichedBinaryReader"/> stream.</summary>
    public sealed class BinaryReaderException : Exception
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
