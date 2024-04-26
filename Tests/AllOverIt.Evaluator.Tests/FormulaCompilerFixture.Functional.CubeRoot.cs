using System;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalCubeRoot : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Known_Result()
        {
            AssertFormula("cbrt(8)", () => 2);
        }

        [Fact]
        public void Should_Return_Cube_Root()
        {
            AssertFormula($"cbrt({Val1})", () => Math.Cbrt(Val1));
        }
    }
}