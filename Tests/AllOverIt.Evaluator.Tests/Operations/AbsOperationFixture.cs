using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AbsOperationFixture : UnaryOperationFixtureBase<AbsOperation>
    {
        protected override Type OperatorType => typeof(AbsOperator);
    }
}