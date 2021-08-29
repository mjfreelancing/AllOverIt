using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalSquareRoot : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Known_Result()
        {
            AssertFormula("sqrt(9)", () => 3);
        }

        [Fact]
        public void Should_Return_Cube_Root()
        {
            AssertFormula($"sqrt({Val1})", () => Math.Sqrt(Val1));
        }
    }
}