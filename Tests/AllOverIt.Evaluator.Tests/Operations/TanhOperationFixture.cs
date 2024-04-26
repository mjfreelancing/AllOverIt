using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class TanhOperationFixture : UnaryOperationFixtureBase<TanhOperation>
    {
        protected override Type OperatorType => typeof(TanhOperator);
    }
}