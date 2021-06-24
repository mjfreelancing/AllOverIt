using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class CosOperationFixture : UnaryOperationFixtureBase<CosOperation>
    {
        protected override Type OperatorType => typeof(CosOperator);
    }
}