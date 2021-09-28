using System;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class CeilingOperationFixture : UnaryOperationFixtureBase<CeilingOperation>
    {
        protected override Type OperatorType => typeof(CeilingOperator);
    }
}