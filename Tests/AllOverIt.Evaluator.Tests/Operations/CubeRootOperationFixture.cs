using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class CubeRootOperationFixture : UnaryOperationFixtureBase<CubeRootOperation>
    {
        protected override Type OperatorType => typeof(CubeRootOperator);
    }
}