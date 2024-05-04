using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class RoundOperationFixture : BinaryOperationFixtureBase<RoundOperation>
    {
        protected override Type OperatorType => typeof(RoundOperator);
    }
}
