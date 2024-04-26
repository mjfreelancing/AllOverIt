using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class NotEqualOperationFixture : BinaryOperationFixtureBase<NotEqualOperation>
    {
        protected override Type OperatorType => typeof(NotEqualOperator);
    }
}