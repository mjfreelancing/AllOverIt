using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiSinOperationFixture : AoiUnaryOperationFixtureBase<AoiSinOperation>
    {
        protected override Type OperatorType => typeof(AoiSinOperator);
    }
}