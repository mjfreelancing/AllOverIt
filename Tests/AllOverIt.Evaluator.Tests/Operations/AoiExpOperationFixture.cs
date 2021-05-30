using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiExpOperationFixture : AoiUnaryOperationFixtureBase<AoiExpOperation>
    {
        protected override Type OperatorType => typeof(AoiExpOperator);
    }
}
