using System;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalCeiling : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Ceiling()
        {
            AssertFormula($"ceil({Val1})", () => Math.Ceiling(Val1));
        }

        [Fact]
        public void Should_Return_Ceiling_When_Negative()
        {
            AssertFormula($"ceil({-Val1})", () => Math.Ceiling(-Val1));
        }
    }
}