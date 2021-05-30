using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiSinhOperationFixture : AoiUnaryOperationFixtureBase<AoiSinhOperation>
    {
        protected override Type OperatorType => typeof(AoiSinhOperator);
    }
}