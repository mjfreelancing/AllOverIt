using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AtanOperationFixture : UnaryOperationFixtureBase<AtanOperation>
    {
        protected override Type OperatorType => typeof(AtanOperator);
    }
}