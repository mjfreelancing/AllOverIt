using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // An operation used to calculate the angle (in radians) of a cosine value.
    public sealed class AcosOperation : ArithmeticOperationBase
    {
        public AcosOperation() 
            : base(1, MakeOperator)
        {
        }

        internal static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new AcosOperator(e[0]));
        }
    }
}