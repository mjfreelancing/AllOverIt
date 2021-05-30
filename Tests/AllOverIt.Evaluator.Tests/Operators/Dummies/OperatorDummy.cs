using AllOverIt.Evaluator.Operators;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators.Dummies
{
    internal class OperatorDummy : AoiOperator<Func<Expression>>
    {
        public OperatorDummy(Func<Expression> operatorType)
            : base(operatorType)
        {
        }

        public override Expression GetExpression()
        {
            return OperatorType.Invoke();
        }
    }
}
