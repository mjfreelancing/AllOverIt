using AllOverIt.Assertion;

namespace AllOverIt.Evaluator.Operators
{
    /// <summary>An abstract base class used for implementing unary, binary and custom operators.</summary>
    /// <typeparam name="TType">The type (a Func) used for operating on one or more operands. This func must
    /// accept one or more Expression arguments and return a resultant Expression.</typeparam>
    public abstract class Operator<TType> : OperatorBase where TType : class
    {
        /// <summary>The object type implementing the custom operator.</summary>
        protected TType OperatorType { get; }

        /// <summary>Constructor.</summary>
        /// <param name="operatorType">The operator type.</param>
        protected Operator(TType operatorType)
        {
            OperatorType = operatorType.WhenNotNull(nameof(operatorType));
        }
    }
}
