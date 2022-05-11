using System;

namespace AllOverIt.Formatters.Objects.Exceptions
{
    /// <summary>Represents an error raised during object property serialization.</summary>
    public class ObjectPropertySerializerException : Exception
    {
        /// <summary>Default constructor.</summary>
        public ObjectPropertySerializerException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public ObjectPropertySerializerException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ObjectPropertySerializerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}