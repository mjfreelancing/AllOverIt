using System;

namespace AllOverIt.Exceptions
{
    /// <summary>Represents an error that occurred while attempting to interpret a name or value as a <see cref="Patterns.Enumeration.RichEnum{TType}"/>.</summary>
    public class RichEnumException : Exception
    {
        /// <summary>Default constructor.</summary>
        public RichEnumException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public RichEnumException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RichEnumException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}