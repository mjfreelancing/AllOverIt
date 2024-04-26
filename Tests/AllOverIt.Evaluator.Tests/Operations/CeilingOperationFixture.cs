using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class CeilingOperationFixture : UnaryOperationFixtureBase<CeilingOperation>
    {
        protected override Type OperatorType => typeof(CeilingOperator);
    }
}