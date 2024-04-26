using System;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalHyperbolicCosine : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Hyperbolic_Cosine()
        {
            AssertFormula($"cosh({Val1})", () => Math.Cosh(Val1));
        }
    }
}