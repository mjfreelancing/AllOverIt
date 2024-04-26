using AllOverIt.Evaluator.Variables;

namespace AllOverIt.Evaluator.Exceptions
{
    /// <summary>An exception that can be thrown while attempting to build an <see cref="IVariableRegistry"/> via an <see cref="IVariableRegistryBuilder"/>.</summary>
    public class VariableRegistryBuilderException : Exception
    {
        /// <summary>Default constructor.</summary>
        public VariableRegistryBuilderException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public VariableRegistryBuilderException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public VariableRegistryBuilderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
