using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class GreaterThanOrEqualOperationFixture : BinaryOperationFixtureBase<GreaterThanOrEqualOperation>
    {
        protected override Type OperatorType => typeof(GreaterThanOrEqualOperator);
    }
}