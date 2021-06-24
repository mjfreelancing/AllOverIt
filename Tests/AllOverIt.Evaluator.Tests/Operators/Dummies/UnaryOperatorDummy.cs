using AllOverIt.Evaluator.Operators;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators.Dummies
{
    internal class UnaryOperatorDummy : UnaryOperator
    {
        public UnaryOperatorDummy(Func<Expression, Expression> operatorType, Expression operand)
            : base(operatorType, operand)
        {
        }
    }
}
