namespace AllOverIt.Filtering.Exceptions
{
    /// <summary>An exception that can be thrown wihle building a query filter.</summary>
    public sealed class NullNotSupportedException : Exception
    {
        /// <summary>Constructor.</summary>
        public NullNotSupportedException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public NullNotSupportedException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NullNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
