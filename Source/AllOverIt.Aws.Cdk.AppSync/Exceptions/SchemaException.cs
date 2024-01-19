using System;

namespace AllOverIt.Aws.Cdk.AppSync.Exceptions
{
    /// <summary>An error describing a condition that would result in an invalid schema.</summary>
    public sealed class SchemaException : Exception
    {
        /// <summary>Default constructor.</summary>
        public SchemaException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public SchemaException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SchemaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}