using AllOverIt.Evaluator.Operators;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators.Dummies
{
    internal class DummyOperator : Operator<Func<Expression>>
    {
        public DummyOperator(Func<Expression> operatorType)
            : base(operatorType)
        {
        }

        public override Expression GetExpression()
        {
            return OperatorType.Invoke();
        }
    }
}
