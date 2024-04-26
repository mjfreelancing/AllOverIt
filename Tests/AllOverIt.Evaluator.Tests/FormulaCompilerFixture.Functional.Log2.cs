namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalLog2 : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Log2()
        {
            AssertFormula($"log2({Val1})", () => Math.Log2(Val1));
        }
    }
}