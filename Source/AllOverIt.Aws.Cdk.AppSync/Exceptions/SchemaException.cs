using System;

namespace AllOverIt.Aws.Cdk.AppSync.Exceptions
{
    /// <summary>An error describing a condition that would result in an invalid schema.</summary>
    public sealed class SchemaException : Exception
    {
        /// <summary>Constructor.</summary>
        /// <param name="message">The error message.</param>
        public SchemaException(string message)
            : base(message)
        {
        }
    }
}