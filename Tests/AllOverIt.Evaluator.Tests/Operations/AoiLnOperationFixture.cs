using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiLnOperationFixture : AoiUnaryOperationFixtureBase<AoiLnOperation>
    {
        protected override Type OperatorType => typeof(AoiLnOperator);
    }
}
