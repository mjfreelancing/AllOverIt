using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalNotEqual : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_If_True()
        {
            AssertFormula($"IF(NE({Val1},{-Val1}),{Val1},{Val2})", () => Val1);
        }

        [Fact]
        public void Should_Return_If_False()
        {
            AssertFormula($"IF(NE({Val1},{Val1}),{Val1},{Val2})", () => Val2);
        }
    }
}