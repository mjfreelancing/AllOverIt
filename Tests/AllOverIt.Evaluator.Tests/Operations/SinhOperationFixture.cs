using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class SinhOperationFixture : UnaryOperationFixtureBase<SinhOperation>
    {
        protected override Type OperatorType => typeof(SinhOperator);
    }
}