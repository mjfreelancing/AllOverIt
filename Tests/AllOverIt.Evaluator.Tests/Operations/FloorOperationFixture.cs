using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class FloorOperationFixture : UnaryOperationFixtureBase<FloorOperation>
    {
        protected override Type OperatorType => typeof(FloorOperator);
    }
}