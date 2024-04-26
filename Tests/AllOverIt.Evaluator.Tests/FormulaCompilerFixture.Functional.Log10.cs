using System;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalLog10 : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Log10()
        {
            AssertFormula($"log10({Val1})", () => Math.Log10(Val1));
        }
    }
}