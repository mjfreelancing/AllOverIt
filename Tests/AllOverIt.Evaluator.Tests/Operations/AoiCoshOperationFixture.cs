using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiCoshOperationFixture : AoiUnaryOperationFixtureBase<AoiCoshOperation>
    {
        protected override Type OperatorType => typeof(AoiCoshOperator);
    }
}