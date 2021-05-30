using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiSqrtOperationFixture : AoiUnaryOperationFixtureBase<AoiSqrtOperation>
    {
        protected override Type OperatorType => typeof(AoiSqrtOperator);
    }
}
