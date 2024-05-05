namespace AllOverIt.Fixture.Exceptions
{
    /// <summary>The exception raised when an unhandled AggregateException is thrown.</summary>
    public sealed class AggregateAssertionException : Exception
    {
        /// <summary>The unhandled exception to report.</summary>
        public AggregateException? UnhandledException { get; }

        /// <summary>Constructor.</summary>
        public AggregateAssertionException()
        {
        }

        /// <summary>Constructor providing the exception message.</summary>
        /// <param name="message">The message to report.</param>
        public AggregateAssertionException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AggregateAssertionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>Constructor providing the exception message, original aggregate exception, and the inner, unhandled exception.</summary>
        /// <param name="message">The message to report.</param>
        /// <param name="innerException">The aggregate exception that was unhandled.</param>
        /// <param name="unhandledException">The unhandled exception, within the aggregate exception</param>
        public AggregateAssertionException(string message, AggregateException innerException, AggregateException unhandledException)
          : base(message, innerException)
        {
            UnhandledException = unhandledException;
        }
    }
}