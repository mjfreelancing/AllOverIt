using System.Linq.Expressions;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation that accepts a boolean expression and returns a second or third expression based on the true/false result.</summary>
    public sealed class IfOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public IfOperation()
            : base(3, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new IfOperator(e[0], e[1], e[2]));
        }
    }
}