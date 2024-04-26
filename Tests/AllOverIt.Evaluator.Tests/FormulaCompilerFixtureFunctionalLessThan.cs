namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalLessThan : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_If_True()
        {
            AssertFormula($"IF(LT({-Val1},{Val1}),{Val1},{Val2})", () => Val1);
        }

        [Fact]
        public void Should_Return_If_False()
        {
            AssertFormula($"IF(LT({Val1},{-Val1}),{Val1},{Val2})", () => Val2);
        }
    }
}