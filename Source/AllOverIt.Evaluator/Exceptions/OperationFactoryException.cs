using System;
using System.Runtime.Serialization;

namespace AllOverIt.Evaluator.Exceptions
{
    /// <summary>An exception that can be thrown by the ArithmeticOperationFactory while compiling a formula expression.</summary>
    [Serializable]
    public class OperationFactoryException : Exception
    {
        public OperationFactoryException()
        {
        }

        public OperationFactoryException(string message)
            : base(message)
        {
        }

        public OperationFactoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected OperationFactoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
