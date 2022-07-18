using System;

namespace AllOverIt.Patterns.Specification.Exceptions
{
    /// <summary>Represents an error that occurred while attempting to parse the expression of an <see cref="ILinqSpecification{Type}"/>.</summary>
    internal class LinqSpecificationVisitorException : Exception
    {
        public string PartialQueryString { get; }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="partialQueryString">The partially completed query string generated up until the error occurred.</param>
        public LinqSpecificationVisitorException(string message, Exception innerException, string partialQueryString)
            : base(message, innerException)
        {
            PartialQueryString = partialQueryString;
        }
    }
}