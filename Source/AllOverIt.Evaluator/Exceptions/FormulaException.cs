using System;
using System.Runtime.Serialization;

namespace AllOverIt.Evaluator.Exceptions
{
    /// <summary>An exception that can be thrown by the FormulaCompiler while compiling a formula expression.</summary>
    [Serializable]
    public class FormulaException : Exception
    {
        public FormulaException()
        {
        }

        public FormulaException(string message)
            : base(message)
        {
        }

        public FormulaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FormulaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
