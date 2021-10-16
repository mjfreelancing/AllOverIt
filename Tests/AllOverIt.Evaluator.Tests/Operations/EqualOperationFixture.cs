using System;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class EqualOperationFixture : BinaryOperationFixtureBase<EqualOperation>
    {
        protected override Type OperatorType => typeof(EqualOperator);
    }
}