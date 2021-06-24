using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class SinOperationFixture : UnaryOperationFixtureBase<SinOperation>
    {
        protected override Type OperatorType => typeof(SinOperator);
    }
}