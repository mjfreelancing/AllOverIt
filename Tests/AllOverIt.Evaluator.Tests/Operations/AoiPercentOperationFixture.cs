using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System;

namespace AllOverIt.Evaluator.Tests.Operations
{
  public class AoiPercentOperationFixture
    : AoiBinaryOperationFixtureBase<AoiPercentOperation>
  {
    protected override Type OperatorType => typeof(AoiPercentOperator);
  }
}
