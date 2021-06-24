using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class TanOperationFixture : UnaryOperationFixtureBase<TanOperation>
    {
        protected override Type OperatorType => typeof(TanOperator);
    }
}