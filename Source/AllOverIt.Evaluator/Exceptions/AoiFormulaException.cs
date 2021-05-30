using System;
using System.Runtime.Serialization;

namespace AllOverIt.Evaluator.Exceptions
{
    // An exception that can be thrown by an AoiFormulaCompiler while compiling a formula expression.
    [Serializable]
    public class AoiFormulaException : Exception
    {
        public AoiFormulaException()
        {
        }

        public AoiFormulaException(string message)
            : base(message)
        {
        }

        public AoiFormulaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AoiFormulaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
