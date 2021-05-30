using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiAtanOperationFixture : AoiUnaryOperationFixtureBase<AoiAtanOperation>
    {
        protected override Type OperatorType => typeof(AoiAtanOperator);
    }
}