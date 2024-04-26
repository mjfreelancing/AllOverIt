using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class EqualOperationFixture : BinaryOperationFixtureBase<EqualOperation>
    {
        protected override Type OperatorType => typeof(EqualOperator);
    }
}