using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AcosOperationFixture : UnaryOperationFixtureBase<AcosOperation>
    {
        protected override Type OperatorType => typeof(AcosOperator);
    }
}