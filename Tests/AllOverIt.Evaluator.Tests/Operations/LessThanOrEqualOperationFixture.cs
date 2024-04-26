using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class LessThanOrEqualOperationFixture : BinaryOperationFixtureBase<LessThanOrEqualOperation>
    {
        protected override Type OperatorType => typeof(LessThanOrEqualOperator);
    }
}