using AllOverIt.Helpers;

namespace AllOverIt.Evaluator.Operators
{
    // An abstract base class used for implementing unary, binary and custom operators.
    // 'TType' is the type used for operating on one or more operands. This object must accept one or more Expression arguments and return a resultant Expression
    public abstract class Operator<TType> : OperatorBase where TType : class
    {
        // The object type implementing the custom operator.
        internal readonly TType OperatorType;

        protected Operator(TType operatorType)
        {
            OperatorType = operatorType.WhenNotNull(nameof(operatorType));
        }
    }
}
