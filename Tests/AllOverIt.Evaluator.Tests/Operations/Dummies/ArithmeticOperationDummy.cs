using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operations.Dummies
{
    internal class ArithmeticOperationDummy : ArithmeticOperationBase
    {
        public ArithmeticOperationDummy()
            : this(1, e => null)
        {
        }

        public ArithmeticOperationDummy(int argumentCount)
            : this(argumentCount, e => null)
        {
        }

        public ArithmeticOperationDummy(int argumentCount, Func<Expression[], IOperator> creator)
            : base(argumentCount, creator)
        {
        }
    }
}
