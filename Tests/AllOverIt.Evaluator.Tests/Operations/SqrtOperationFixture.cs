using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class SqrtOperationFixture : UnaryOperationFixtureBase<SqrtOperation>
    {
        protected override Type OperatorType => typeof(SqrtOperator);
    }
}
