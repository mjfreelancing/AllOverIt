using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class MaxOperationFixture : BinaryOperationFixtureBase<MaxOperation>
    {
        protected override Type OperatorType => typeof(MaxOperator);
    }
}