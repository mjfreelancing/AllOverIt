namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalLessThanOrEqual : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_If_True_When_Less()
        {
            AssertFormula($"IF(LTE({-Val1},{Val1}),{Val1},{Val2})", () => Val1);
        }

        [Fact]
        public void Should_Return_If_True_When_Equal()
        {
            AssertFormula($"IF(LTE({Val1},{Val1}),{Val1},{Val2})", () => Val1);
        }

        [Fact]
        public void Should_Return_If_False()
        {
            AssertFormula($"IF(LTE({Val1},{-Val1}),{Val1},{Val2})", () => Val2);
        }
    }
}