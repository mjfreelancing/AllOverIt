using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalMaximum : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Maximum()
        {
            AssertFormula($"max({Val1}, {Val2})", () => Math.Max(Val1, Val2));
        }
    }
}