using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class GreaterThanOperationFixture : BinaryOperationFixtureBase<GreaterThanOperation>
    {
        protected override Type OperatorType => typeof(GreaterThanOperator);
    }
}