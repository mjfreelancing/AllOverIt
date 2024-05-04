namespace AllOverIt.Pagination.Exceptions
{
    /// <summary>Represents an error that occurs when configuring or executing a paginated query.</summary>
    public sealed class PaginationException : Exception
    {
        /// <summary>Default constructor.</summary>
        public PaginationException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public PaginationException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PaginationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
