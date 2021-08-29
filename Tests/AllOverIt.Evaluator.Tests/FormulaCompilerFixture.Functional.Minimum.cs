using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalMinimum : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Minimum()
        {
            AssertFormula($"min({Val1}, {Val2})", () => Math.Min(Val1, Val2));
        }
    }
}