using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class PercentOperationFixture : BinaryOperationFixtureBase<PercentOperation>
    {
        protected override Type OperatorType => typeof(PercentOperator);
    }
}
