namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalEqual : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_If_True()
        {
            AssertFormula($"IF(EQ({Val1},{Val1}),{Val1},{Val2})", () => Val1);
        }

        [Fact]
        public void Should_Return_If_False()
        {
            AssertFormula($"IF(EQ({Val1},{-Val1}),{Val1},{Val2})", () => Val2);
        }
    }
}