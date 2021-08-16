using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the cosine of an angle (in radians).
    public sealed class CosOperation : ArithmeticOperationBase
    {
        public CosOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new CosOperator(e[0]));
        }
    }
}