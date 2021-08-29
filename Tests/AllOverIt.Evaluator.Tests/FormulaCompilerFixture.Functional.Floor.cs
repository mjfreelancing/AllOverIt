using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalFloor : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Floor()
        {
            AssertFormula($"floor({Val1})", () => Math.Floor(Val1));
        }

        [Fact]
        public void Should_Return_Floor_When_Negative()
        {
            AssertFormula($"floor({-Val1})", () => Math.Floor(-Val1));
        }
    }
}