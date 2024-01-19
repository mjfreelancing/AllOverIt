using System;
using AllOverIt.Evaluator.Variables;

namespace AllOverIt.Evaluator.Exceptions
{
    /// <summary>An exception that can be thrown by a concrete IVariable.</summary>
    public class VariableException : Exception
    {
        /// <summary>Default constructor.</summary>
        public VariableException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public VariableException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public VariableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
