using System;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalHyperbolicSine : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Hyperbolic_Sine()
        {
            AssertFormula($"sinh({Val1})", () => Math.Sinh(Val1));
        }
    }
}