using AllOverIt.Evaluator.Operators;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators.Dummies
{
    internal class OperatorBaseDummy1 : AoiOperatorBase
    {
        public OperatorBaseDummy1(Expression operand1, Expression operand2)
        {
        }

        public override Expression GetExpression()
        {
            throw new NotImplementedException();
        }
    }
}
