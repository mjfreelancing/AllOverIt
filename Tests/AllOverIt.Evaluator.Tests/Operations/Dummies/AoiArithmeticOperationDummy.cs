using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operations.Dummies
{
    internal class AoiArithmeticOperationDummy : AoiArithmeticOperationBase
    {
        public AoiArithmeticOperationDummy()
            : this(1, e => null)
        {
        }

        public AoiArithmeticOperationDummy(int argumentCount)
            : this(argumentCount, e => null)
        {
        }

        public AoiArithmeticOperationDummy(int argumentCount, Func<Expression[], IAoiOperator> creator)
            : base(argumentCount, creator)
        {
        }
    }
}
