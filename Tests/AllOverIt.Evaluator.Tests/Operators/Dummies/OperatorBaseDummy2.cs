using AllOverIt.Evaluator.Operators;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators.Dummies
{
    internal class OperatorBaseDummy2 : OperatorBase
    {
        public OperatorBaseDummy2(Expression operand1, Expression operand2)
        {
        }

        public OperatorBaseDummy2(Expression operand1, Expression operand2, Expression operand3)
        {
        }

        public override Expression GetExpression()
        {
            throw new NotImplementedException();
        }
    }
}
