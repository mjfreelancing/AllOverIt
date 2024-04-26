using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class IfOperationFixture : TernaryOperationFixtureBase<IfOperation>
    {
        protected override Type OperatorType => typeof(IfOperator);
    }
}