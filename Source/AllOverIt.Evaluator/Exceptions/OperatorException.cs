using System;
using System.Runtime.Serialization;

namespace AllOverIt.Evaluator.Exceptions
{
    /// <summary>An exception that can be thrown by a concrete IOperator while compiling a formula expression.</summary>
    [Serializable]
    public class OperatorException : Exception
    {
        public OperatorException()
        {
        }

        public OperatorException(string message)
            : base(message)
        {
        }

        public OperatorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected OperatorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
