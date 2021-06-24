using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using CustomOperation.Operators;
using System.Linq.Expressions;

namespace CustomOperation.Operations
{
    public sealed class CustomMinOperation : ArithmeticOperationBase
    {
        public CustomMinOperation()
            : base(2, MakeOperator)
        {
        }

        public static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new CustomMinOperator(e[0], e[1]));
        }
    }
}