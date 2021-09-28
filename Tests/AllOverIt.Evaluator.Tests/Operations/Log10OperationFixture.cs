using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class Log10OperationFixture : UnaryOperationFixtureBase<Log10Operation>
    {
        protected override Type OperatorType => typeof(Log10Operator);
    }
}
