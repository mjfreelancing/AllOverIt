using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators.Dummies
{
    internal class DummyOperatorBase1 : OperatorBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Can't have more than one discard, so keep them both")]
        public DummyOperatorBase1(Expression operand1, Expression operand2)
        {
        }

        public override Expression GetExpression()
        {
            throw new NotImplementedException();
        }
    }
}
