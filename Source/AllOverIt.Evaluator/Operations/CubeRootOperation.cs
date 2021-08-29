using System.Linq.Expressions;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>An operation used to calculate the cube root of a number.</summary>
    public sealed class CubeRootOperation : ArithmeticOperationBase
    {
        /// <summary>Constructor.</summary>
        public CubeRootOperation()
            : base(1, MakeOperator)
        {
        }

        private static IOperator MakeOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new CubeRootOperator(e[0]));
        }
    }
}