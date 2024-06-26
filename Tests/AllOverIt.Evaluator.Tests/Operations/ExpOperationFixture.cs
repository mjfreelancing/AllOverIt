using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class ExpOperationFixture : UnaryOperationFixtureBase<ExpOperation>
    {
        protected override Type OperatorType => typeof(ExpOperator);
    }
}
