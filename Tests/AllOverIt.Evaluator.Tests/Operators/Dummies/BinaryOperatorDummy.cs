using AllOverIt.Evaluator.Operators;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators.Dummies
{
    internal class BinaryOperatorDummy : BinaryOperator
    {
        public BinaryOperatorDummy(Func<Expression, Expression, Expression> operatorType, Expression leftOperand, Expression rightOperand)
            : base(operatorType, leftOperand, rightOperand)
        {
        }
    }
}
