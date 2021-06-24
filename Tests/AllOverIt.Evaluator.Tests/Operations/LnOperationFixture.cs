using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class LnOperationFixture : UnaryOperationFixtureBase<LnOperation>
    {
        protected override Type OperatorType => typeof(LnOperator);
    }
}
