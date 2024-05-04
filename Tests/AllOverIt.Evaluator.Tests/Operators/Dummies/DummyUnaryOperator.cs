using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators.Dummies
{
    internal class DummyUnaryOperator : UnaryOperator
    {
        public DummyUnaryOperator(Func<Expression, Expression> operatorType, Expression operand)
            : base(operatorType, operand)
        {
        }
    }
}
