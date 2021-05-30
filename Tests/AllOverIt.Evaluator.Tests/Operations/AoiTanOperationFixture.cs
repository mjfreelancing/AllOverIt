using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiTanOperationFixture : AoiUnaryOperationFixtureBase<AoiTanOperation>
    {
        protected override Type OperatorType => typeof(AoiTanOperator);
    }
}