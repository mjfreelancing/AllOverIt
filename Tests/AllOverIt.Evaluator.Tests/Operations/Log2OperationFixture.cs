using System;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class Log2OperationFixture : UnaryOperationFixtureBase<Log2Operation>
    {
        protected override Type OperatorType => typeof(Log2Operator);
    }
}