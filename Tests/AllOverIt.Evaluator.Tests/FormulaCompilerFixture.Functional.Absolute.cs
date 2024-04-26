using System;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalAbsolute : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Absolute()
        {
            AssertFormula($"abs({Val1})", () => Math.Abs(Val1));
        }

        [Fact]
        public void Should_Return_Absolute_When_Negative()
        {
            AssertFormula($"abs({-Val1})", () => Math.Abs(-Val1));
        }
    }
}