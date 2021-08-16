using System;

namespace AllOverIt.Evaluator.Exceptions
{
    /// <summary>An exception that can be thrown by the FormulaCompiler while compiling a formula expression.</summary>
    public class FormulaException : Exception
    {
        /// <summary>Default constructor.</summary>
        public FormulaException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public FormulaException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FormulaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
