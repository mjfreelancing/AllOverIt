using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiCosOperationFixture : AoiUnaryOperationFixtureBase<AoiCosOperation>
    {
        protected override Type OperatorType => typeof(AoiCosOperator);
    }
}