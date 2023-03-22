﻿using System;
using System.Linq.Expressions;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operators.Dummies
{
    internal class DummyTernaryOperator : TernaryOperator
    {
        public DummyTernaryOperator(Func<Expression, Expression, Expression, Expression> operatorType, Expression operand1, Expression operand2,
            Expression operand3)
            : base(operatorType, operand1, operand2, operand3)
        {
        }
    }
}