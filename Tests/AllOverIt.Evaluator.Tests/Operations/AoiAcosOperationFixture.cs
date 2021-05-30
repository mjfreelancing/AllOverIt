using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiAcosOperationFixture : AoiUnaryOperationFixtureBase<AoiAcosOperation>
    {
        protected override Type OperatorType => typeof(AoiAcosOperator);
    }
}