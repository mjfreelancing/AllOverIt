using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators.Dummies
{
    internal class DummyOperatorBase2 : OperatorBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Can't have more than one discard so keep them both")]
        public DummyOperatorBase2(Expression operand1, Expression operand2)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Can't have more than one discard so keep them both")]
        public DummyOperatorBase2(Expression operand1, Expression operand2, Expression operand3)
        {
        }

        public override Expression GetExpression()
        {
            throw new NotImplementedException();
        }
    }
}
