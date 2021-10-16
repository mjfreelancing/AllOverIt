using AllOverIt.Evaluator.Operators;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators.Dummies
{
    internal class BinaryOperatorDummy : BinaryOperator
    {
        public BinaryOperatorDummy(Func<Expression, Expression, Expression> operatorType, Expression operand1, Expression operand2)
            : base(operatorType, operand1, operand2)
        {
        }
    }
}
