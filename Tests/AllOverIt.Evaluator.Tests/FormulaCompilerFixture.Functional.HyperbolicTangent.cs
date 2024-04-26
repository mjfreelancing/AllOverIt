using System;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalHyperbolicTangent : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Hyperbolic_Tangent()
        {
            AssertFormula($"tanh({Val1})", () => Math.Tanh(Val1));
        }
    }
}