using System;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalSine : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Sine()
        {
            AssertFormula($"sin({Val1})", () => Math.Sin(Val1));
        }
    }
}