using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using CustomOperationDemo.Operators;
using System.Linq.Expressions;

namespace CustomOperationDemo.Operations
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