using System;

namespace AllOverIt.Csv.Exceptions
{
    /// <summary>Represents an error that occurs during the serialization of data to CSV.</summary>
    public sealed class CsvSerializerException : Exception
    {
        /// <summary>Default constructor.</summary>
        public CsvSerializerException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public CsvSerializerException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CsvSerializerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}