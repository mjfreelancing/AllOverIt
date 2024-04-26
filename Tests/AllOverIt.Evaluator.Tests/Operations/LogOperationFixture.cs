using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class LogOperationFixture : UnaryOperationFixtureBase<LogOperation>
    {
        protected override Type OperatorType => typeof(LogOperator);
    }
}
