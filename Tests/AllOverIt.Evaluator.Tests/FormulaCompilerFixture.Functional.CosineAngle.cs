using System;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalCosineAngle : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Cosine()
        {
            var angle = Math.Cos(Val1);

            AssertFormula($"acos({angle})", () => Math.Acos(angle));
        }
    }
}