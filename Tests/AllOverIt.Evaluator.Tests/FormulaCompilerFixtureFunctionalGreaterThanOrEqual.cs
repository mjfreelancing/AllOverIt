using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalGreaterThanOrEqual : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_If_True_When_Greater()
        {
            AssertFormula($"IF(GTE({Val1},{-Val1}),{Val1},{Val2})", () => Val1);
        }

        [Fact]
        public void Should_Return_If_True_When_Equal()
        {
            AssertFormula($"IF(GTE({Val1},{Val1}),{Val1},{Val2})", () => Val1);
        }

        [Fact]
        public void Should_Return_If_False()
        {
            AssertFormula($"IF(GTE({-Val1},{Val1}),{Val1},{Val2})", () => Val2);
        }
    }
}