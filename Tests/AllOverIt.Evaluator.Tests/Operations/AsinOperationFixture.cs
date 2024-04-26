using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AsinOperationFixture : UnaryOperationFixtureBase<AsinOperation>
    {
        protected override Type OperatorType => typeof(AsinOperator);
    }
}