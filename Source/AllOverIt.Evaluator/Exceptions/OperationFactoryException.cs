using System;

namespace AllOverIt.Evaluator.Exceptions
{
    /// <summary>An exception that can be thrown by the ArithmeticOperationFactory while compiling a formula expression.</summary>
    public class OperationFactoryException : Exception
    {
        /// <summary>Default constructor.</summary>
        public OperationFactoryException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public OperationFactoryException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public OperationFactoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
