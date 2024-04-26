namespace AllOverIt.Evaluator.Exceptions
{
    /// <summary>An exception that can be thrown by a concrete IOperator while compiling a formula expression.</summary>
    public sealed class OperatorException : Exception
    {
        /// <summary>Default constructor.</summary>
        public OperatorException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public OperatorException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public OperatorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
