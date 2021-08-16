using System;

namespace AllOverIt.Evaluator.Exceptions
{
    /// <summary>An exception that can be thrown to indicate a concrete IVariable instance is not mutable.</summary>
    public class VariableImmutableException : VariableException
    {
        /// <summary>Default constructor.</summary>
        public VariableImmutableException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public VariableImmutableException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public VariableImmutableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
