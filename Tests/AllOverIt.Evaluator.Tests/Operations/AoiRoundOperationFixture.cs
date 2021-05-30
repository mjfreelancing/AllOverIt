using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiRoundOperationFixture : AoiBinaryOperationFixtureBase<AoiRoundOperation>
    {
        protected override Type OperatorType => typeof(AoiRoundOperator);
    }
}
