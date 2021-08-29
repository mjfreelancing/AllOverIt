using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class LnOperationFixture : UnaryOperationFixtureBase<LogOperation>
    {
        protected override Type OperatorType => typeof(LogOperator);
    }
}
