using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class CoshOperationFixture : UnaryOperationFixtureBase<CoshOperation>
    {
        protected override Type OperatorType => typeof(CoshOperator);
    }
}