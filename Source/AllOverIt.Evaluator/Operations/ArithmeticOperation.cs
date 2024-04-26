using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>Represents an arithmetic operator or a user-defined method.</summary>
    public sealed class ArithmeticOperation : ArithmeticOperationBase
    {
        /// <summary>The precedence that determines the order of operations. A lower precedence value indicates a higher priority
        /// (refer to http://en.wikipedia.org/wiki/Order_of_operations).</summary>
        public int Precedence { get; }

        /// <summary>Constructor.</summary>
        /// <param name="precedence">The precedence that determines the order of operations.</param>
        /// <param name="argumentCount">The number of required arguments for the operator or user-defined method.</param>
        /// <param name="creator"></param>
        public ArithmeticOperation(int precedence, int argumentCount, Func<Expression[], IOperator> creator)
          : base(argumentCount, creator)
        {
            Precedence = precedence;
        }
    }
}