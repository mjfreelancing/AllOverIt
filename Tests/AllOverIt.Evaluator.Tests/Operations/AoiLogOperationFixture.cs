using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiLogOperationFixture : AoiUnaryOperationFixtureBase<AoiLogOperation>
    {
        protected override Type OperatorType => typeof(AoiLogOperator);
    }
}
